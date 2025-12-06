using Microsoft.Extensions.Logging;

using Application.Shared.Data;
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

        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        
        var verificationCode = _codesClient.GenerateCode();
        
        _logger.LogDebug("Saving restore code {verificationCode} to storage", verificationCode);
        var storeTask = _codesClient.StoreRestoreTokenAsync(user.Email, verificationCode);
        
        _logger.LogDebug("Sending restore code {code} to email {email}", verificationCode, user.Email);
        var sendTask = _codesClient.SendRestoreTokenAsync(user.Email, verificationCode, cancellationToken);
        
        await Task.WhenAll(storeTask, sendTask).ConfigureAwait(false);
        
        return Result.Success();
    }
}