using Microsoft.Extensions.Logging;

using Domain.Events;
using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.Users.Events.UserRegistered;

public class UserRegisteredEventHandler : IDomainEventHandler<UserRegisteredEvent>
{
    private readonly ICodesClient _codesClient;
    private readonly ILogger<UserRegisteredEvent> _logger;

    public UserRegisteredEventHandler(ICodesClient codesClient, ILogger<UserRegisteredEvent> logger)
    {
        _logger = logger;
        _codesClient = codesClient;
    }

    public async Task HandleAsync(UserRegisteredEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Handling user {userId} registered event",  @event.UserId);
        
        var verificationCode = _codesClient.GenerateCode();
        _logger.LogDebug("Saving verification code {verificationCode} to storage", verificationCode);
        var storeTask = _codesClient.StoreConfirmationCodeAsync(@event.Email, verificationCode);
        
        var sendTask = _codesClient.SendConfirmationCodeAsync(@event.Email, verificationCode, cancellationToken);
        _logger.LogDebug("Sending verification code {code} to email {email}", verificationCode, @event.Email);
        
        await Task.WhenAll(storeTask, sendTask).ConfigureAwait(false);
    }
}