using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Domain.Common;

namespace Application.Shared.Messaging;

public sealed class InMemoryDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InMemoryDomainEventDispatcher> _logger;
    
    private static readonly ConcurrentDictionary<Type, Func<object, IDomainEvent, CancellationToken, Task>> InvokerCache = new();

    public InMemoryDomainEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<InMemoryDomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var eventType = domainEvent.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
        var handler = _serviceProvider.GetRequiredService(handlerType);
        
        var invoker = InvokerCache.GetOrAdd(eventType, CreateCompiledInvoker);
        
        try
        {
            await invoker(handler, domainEvent, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Error in {HandlerType} while handling {EventType}",
                handler.GetType().Name, eventType.Name);
            throw;
        }
    }

    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        var tasks = domainEvents.Select(domainEvent => DispatchAsync(domainEvent, cancellationToken));
        return Task.WhenAll(tasks);
    }
    
    // Creates a cached invoker
    private static Func<object, IDomainEvent, CancellationToken, Task> CreateCompiledInvoker(Type eventType)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
        var method = handlerType.GetMethod("HandleAsync")!;
        
        // Delegate params: (object handler, IDomainEvent event, CancellationToken ct)
        var handlerParam = Expression.Parameter(typeof(object), "handler");
        var eventParam = Expression.Parameter(typeof(IDomainEvent), "event");
        var ctParam = Expression.Parameter(typeof(CancellationToken), "ct");

        // Type case: ((IDomainEventHandler<T>)handler).HandleAsync((T)event, ct)
        var castHandler = Expression.Convert(handlerParam, handlerType);
        var castEvent = Expression.Convert(eventParam, eventType);
        
        var methodCall = Expression.Call(castHandler, method, castEvent, ctParam);

        return Expression
            .Lambda<Func<object, IDomainEvent, CancellationToken, Task>>(
                methodCall, handlerParam, eventParam, ctParam)
            .Compile();
    }
}