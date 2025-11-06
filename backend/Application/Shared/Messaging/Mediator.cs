using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentValidation;

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
        await ValidateCommand(command, cancellationToken);
        
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
        
        return await task;
    }

    public async Task Send(ICommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Sending command {command}", command);
        await ValidateCommand(command, cancellationToken);
        
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("Handle")
            ?? throw new InvalidOperationException($"No handler found for {command.GetType()}");
        
        _logger.LogDebug("Handling command {command}", command);
        var result = method.Invoke(handler, [command, cancellationToken]);
        
        if (result is not Task task)
        {
            throw new InvalidOperationException($"Command handler {command.GetType()} did not return task");
        }
        
        await task;    
    }

    public async Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Sending query {query}", query);
        await ValidateQuery(query, cancellationToken);
        
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
        
        return await task;
    }
    
    private async Task ValidateCommand(ICommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Validating command {command}", command.GetType().Name);
        var validatorType = typeof(IValidator<>).MakeGenericType(command.GetType());
        var validator = _serviceProvider.GetService(validatorType);
        
        if (validator != null)
        {
            var validationResult = await ((IValidator)validator).ValidateAsync(new ValidationContext<object>(command), cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
    
    private async Task ValidateCommand<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Validating command {command}", command.GetType().Name);
        var validatorType = typeof(IValidator<>).MakeGenericType(command.GetType());
        var validator = _serviceProvider.GetService(validatorType);

        if (validator != null)
        {
            var validationResult = await ((IValidator)validator).ValidateAsync(new ValidationContext<object>(command), cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
    
    private async Task ValidateQuery<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Validating query {command}", query.GetType().Name);
        var validatorType = typeof(IValidator<>).MakeGenericType(query.GetType());
        var validator = _serviceProvider.GetService(validatorType);
        
        if (validator != null)
        {
            var validationResult = await ((IValidator)validator).ValidateAsync(new ValidationContext<object>(query), cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}
