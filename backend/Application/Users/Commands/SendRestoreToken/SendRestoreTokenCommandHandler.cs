using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Errors;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Commands.SendRestoreToken;

public class SendRestoreTokenCommandHandler : ICommandHandler<SendRestoreTokenCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ICodesClient _codesClient;
    private readonly ILogger<SendRestoreTokenCommandHandler> _logger;

    public SendRestoreTokenCommandHandler(
        IUsersRepository usersRepository,
        ICodesClient codesClient,
        ILogger<SendRestoreTokenCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _codesClient = codesClient;
        _logger = logger;
    }

    public async Task<Result> Handle(SendRestoreTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository
            .GetByEmail(request.Email, cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
        {
            _logger.LogError("Tried to send restore code but user with email {Email} doesnt exist.", request.Email);
            return Result.Failure(UserErrors.NotFound);
        }

        if (!user.IsActive)
        {
            _logger.LogError(
                "User {UserId} tried to request sending code to their email {Email} but user is not active.",
                user.Id, request.Email);
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        var restoreCode = _codesClient.GenerateCode();
        
        _logger.LogDebug("Saving restore code for {UserId} to storage.", user.Id);
        var storeTask = _codesClient.StoreRestoreTokenAsync(user.Email, restoreCode);
        
        _logger.LogDebug("Sending restore code for {UserId} to email {Email}.", user.Id, user.Email);
        var sendTask = _codesClient.SendRestoreTokenAsync(user.Email, restoreCode, cancellationToken);
        
        await Task.WhenAll(storeTask, sendTask).ConfigureAwait(false);
        
        _logger.LogInformation("Successfully sent restore code for {UserId} to email {Email}.", user.Id, user.Email);
        
        return Result.Success();
    }
}