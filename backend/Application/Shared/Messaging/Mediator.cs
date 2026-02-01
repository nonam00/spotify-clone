using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentValidation;
using Application.Shared.Errors;
using Domain.Common;

namespace Application.Shared.Messaging;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Mediator> _logger;

    public Mediator(IServiceProvider serviceProvider, ILogger<Mediator> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Sending command {command}", command);
        var validationError = await ValidateCommand(command, cancellationToken).ConfigureAwait(false);
        
        if (validationError != null)
        {
            return HandleValidationError<TResponse>(validationError);
        }
        
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        
        var method = handlerType.GetMethod("Handle") 
            ?? throw new InvalidOperationException($"No handler found for {command.GetType()}");
        
        _logger.LogDebug("Handling command {command}", command);
        var result = method.Invoke(handler, [command, cancellationToken]);
        
        if (result is not Task<TResponse> task)
        {
            throw new InvalidOperationException($"Command handler {command.GetType()} did not return task");
        }
        
        return await task.ConfigureAwait(false);
    }

    public async Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Sending query {query}", query);
        
        var validationError = await ValidateQuery(query, cancellationToken).ConfigureAwait(false);
        
        if (validationError != null)
        {
            return HandleValidationError<TResponse>(validationError);
        }
        
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        
        var method = handlerType.GetMethod("Handle")
            ?? throw new InvalidOperationException($"No handler found for {query.GetType()}");
        
        _logger.LogDebug("Handling query {query}", query);
        var result = method.Invoke(handler, [query, cancellationToken]);

        if (result is not Task<TResponse> task)
        {
            throw new InvalidOperationException($"Query handler {query.GetType()} did not return task");
        }
        
        return await task.ConfigureAwait(false);
    }
    
    
    private async Task<Error?> ValidateCommand<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Validating command {command}", command.GetType().Name);
        
        var validatorType = typeof(IValidator<>).MakeGenericType(command.GetType());
        var validatorObject = _serviceProvider.GetService(validatorType);

        if (validatorObject is not IValidator validator) 
        {
            return null;
        }
        
        var validationResult = await validator
            .ValidateAsync(new ValidationContext<object>(command), cancellationToken)
            .ConfigureAwait(false);
        
        return !validationResult.IsValid
            ? ValidationErrors.CreateFromFluentValidation(validationResult)
            : null;
    }
    
    private async Task<Error?> ValidateQuery<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Validating query {command}", query.GetType().Name);
        var validatorType = typeof(IValidator<>).MakeGenericType(query.GetType());
        var validatorObject = _serviceProvider.GetService(validatorType);

        if (validatorObject is not IValidator validator) 
        {
            return null;
        }

        var validationResult = await validator.ValidateAsync(new ValidationContext<object>(query), cancellationToken).ConfigureAwait(false);
        
        return !validationResult.IsValid ?
            ValidationErrors.CreateFromFluentValidation(validationResult)
            : null;
    }
    
    private static TResponse HandleValidationError<TResponse>(Error validationError)
    {
        var responseType = typeof(TResponse);
        
        // Check if TResponse is Result
        if (responseType == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(validationError);
        }
        
        // Check if TResponse is Result<T>
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var genericResultType = typeof(Result<>).MakeGenericType(valueType);
            
            // Get the Failure method from the generic Result<T> type
            var failureMethod = genericResultType.GetMethod(
                "Failure", 
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.Static | 
                System.Reflection.BindingFlags.DeclaredOnly);
            
            if (failureMethod != null)
            {
                var result = failureMethod.Invoke(null, [validationError]);
                return (TResponse)(result ?? throw new InvalidOperationException("Failed to create Result.Failure"));
            }
        }
        
        // If TResponse is not Result or Result<T>, throw exception for backward compatibility
        throw new ValidationException($"Validation failed: {validationError.Description}");
    }
}
