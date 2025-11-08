using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.Users.Events.UserRegistered;

public class UserRegisteredEventHandler : IDomainEventHandler<UserRegisteredEvent>
{
    private readonly IEmailVerificator _emailVerificator;
    private readonly ILogger<UserRegisteredEvent> _logger;

    public UserRegisteredEventHandler(IEmailVerificator emailVerificator, ILogger<UserRegisteredEvent> logger)
    {
        _logger = logger;
        _emailVerificator = emailVerificator;
    }

    public async Task HandleAsync(UserRegisteredEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Handling user {userId} registered event",  @event.UserId);
        
        var verificationCode = _emailVerificator.GenerateCode();
        _logger.LogDebug("Saving verification code {verificationCode} to storage", verificationCode);
        var storeTask = _emailVerificator.StoreCodeAsync(@event.Email, verificationCode);
        
        var sendTask = _emailVerificator.SendCodeAsync(@event.Email, verificationCode, cancellationToken);
        _logger.LogDebug("Sending verification code {code} to email {email}", verificationCode, @event.Email);
        
        await Task.WhenAll(storeTask, sendTask).ConfigureAwait(false);
    }
}