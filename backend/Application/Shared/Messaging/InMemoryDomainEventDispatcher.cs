using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Domain.Common;

namespace Application.Shared.Messaging;

public class InMemoryDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InMemoryDomainEventDispatcher> _logger;

    public InMemoryDomainEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<InMemoryDomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var eventType = domainEvent.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        try
        {
            var handlers = _serviceProvider.GetServices(handlerType);
            
            foreach (var handler in handlers)
            {
                _logger.LogDebug("Dispatching domain event {EventType} to handler {HandlerType}", 
                    eventType.Name, handler?.GetType().Name);

                var handleMethod = handlerType.GetMethod("HandleAsync");
                if (handleMethod != null)
                {
                    var task = (Task)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!;
                    await task.ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dispatching domain event {EventType}", eventType.Name);
            throw;
        }
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        foreach (var domainEvent in domainEvents)
        {
            await DispatchAsync(domainEvent, cancellationToken);
        }
    }
}