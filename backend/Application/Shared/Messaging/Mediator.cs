using FluentValidation;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

using Domain.Common;
using Application.Shared.Errors;

namespace Application.Shared.Messaging;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    private static readonly ConcurrentDictionary<Type, Func<object, object, CancellationToken, Task>> HandlerInvokers = new();
    private static readonly ConcurrentDictionary<Type, Func<Error, object>> FailureFactories = new();
    private static readonly ConcurrentDictionary<Type, Func<IServiceProvider, object, CancellationToken, Task<Error?>>> ValidationInvokers = new();

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        => ProcessRequest<TResponse>(command, cancellationToken);

    public Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        => ProcessRequest<TResponse>(query, cancellationToken);

    private async Task<TResponse> ProcessRequest<TResponse>(object request, CancellationToken cancellationToken)
    {
        var requestType = request.GetType();

        // Validation
        var validationInvoker = ValidationInvokers.GetOrAdd(requestType, CreateValidationInvoker);
        var validationError = await validationInvoker(_serviceProvider, request, cancellationToken).ConfigureAwait(false);
        
        if (validationError != null)
        {
            var failureFactory = FailureFactories.GetOrAdd(typeof(TResponse), CreateFailureFactory);
            return (TResponse)failureFactory(validationError);
        }

        // Handling
        var invoker = HandlerInvokers.GetOrAdd(requestType, t => CreateHandlerInvoker(t, typeof(TResponse)));
        
        var handlerType = GetHandlerInterface(requestType, typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        
        return await ((Task<TResponse>)invoker(handler, request, cancellationToken)).ConfigureAwait(false);
    }

    private static Func<object, object, CancellationToken, Task> CreateHandlerInvoker(Type requestType, Type responseType)
    {
        var handlerInterface = GetHandlerInterface(requestType, responseType);
        var method = handlerInterface.GetMethod("Handle")!;

        var handlerParam = Expression.Parameter(typeof(object), "h");
        var requestParam = Expression.Parameter(typeof(object), "r");
        var ctParam = Expression.Parameter(typeof(CancellationToken), "ct");

        var castHandler = Expression.Convert(handlerParam, handlerInterface);
        var castRequest = Expression.Convert(requestParam, requestType);
        
        var call = Expression.Call(castHandler, method, castRequest, ctParam);
        
        return Expression.Lambda<Func<object, object, CancellationToken, Task>>(call, handlerParam, requestParam, ctParam).Compile();
    }

    private static Func<IServiceProvider, object, CancellationToken, Task<Error?>> CreateValidationInvoker(Type requestType)
    {
        return async (sp, req, ct) =>
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(requestType);
            var validator = (IValidator?)sp.GetService(validatorType);
            if (validator == null) return null;

            var context = new ValidationContext<object>(req);
            var result = await validator.ValidateAsync(context, ct).ConfigureAwait(false);
            return result.IsValid ? null : ValidationErrors.CreateFromFluentValidation(result);
        };
    }

    private static Func<Error, object> CreateFailureFactory(Type responseType)
    {
        if (responseType == typeof(Result))
        {
            return Result.Failure;
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var method = responseType.GetMethod("Failure", [typeof(Error)])!;
            
            var errParam = Expression.Parameter(typeof(Error), "e");
            var call = Expression.Call(null, method, errParam);
            
            return Expression.Lambda<Func<Error, object>>(call, errParam).Compile();
        }

        return err => throw new ValidationException($"Validation failed: {err.Description}");
    }

    private static Type GetHandlerInterface(Type requestType, Type responseType)
    {
        // Checking if it's query or command handler
        return typeof(ICommand<>).MakeGenericType(responseType).IsAssignableFrom(requestType) 
            ? typeof(ICommandHandler<,>).MakeGenericType(requestType, responseType)
            : typeof(IQueryHandler<,>).MakeGenericType(requestType, responseType);
    }
}