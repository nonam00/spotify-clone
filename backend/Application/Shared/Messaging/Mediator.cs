using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace Application.Shared.Messaging;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        await ValidateCommand(command, cancellationToken);
        
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("Handle")!;
        
        var result = method.Invoke(handler, [command, cancellationToken]);
        return await (Task<TResponse>)result!;
    }

    public async Task Send(ICommand command, CancellationToken cancellationToken = default)
    {
        await ValidateCommand(command, cancellationToken);
        
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("Handle")!;
        
        var result = method.Invoke(handler, [command, cancellationToken]);
        await (Task)result!;
    }

    public async Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        await ValidateQuery(query, cancellationToken);
        
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("Handle")!;
        
        var result = method.Invoke(handler, [query, cancellationToken]);
        return await (Task<TResponse>)result!;
    }
    
    private async Task ValidateCommand(ICommand command, CancellationToken cancellationToken)
    {
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
