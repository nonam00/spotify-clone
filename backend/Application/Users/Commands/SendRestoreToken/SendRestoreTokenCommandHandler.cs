using Application.Shared.Integration;
using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Errors;
using Domain.ValueObjects;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Commands.SendRestoreToken;

public class SendRestoreTokenCommandHandler : ICommandHandler<SendRestoreTokenCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ICodesRepository _codesRepository;
    private readonly IEmailServicePublisher _emailServicePublisher;
    private readonly ILogger<SendRestoreTokenCommandHandler> _logger;

    public SendRestoreTokenCommandHandler(
        IUsersRepository usersRepository,
        ICodesRepository codesClient,
        IEmailServicePublisher emailServicePublisher,
        ILogger<SendRestoreTokenCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _codesRepository = codesClient;
        _emailServicePublisher = emailServicePublisher;
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

        var restoreCode = new UserCode();
        
        _logger.LogDebug("Saving restore code for {UserId} to storage.", user.Id);
        await _codesRepository
            .SetRestoreCode(user.Email, restoreCode, restoreCode.CodeExpiry)
            .ConfigureAwait(false);
        
        _logger.LogDebug("Sending restore code for {UserId} to email {Email}.", user.Id, user.Email);
        await _emailServicePublisher
            .PublishSendRestoreEmail(user.Email, restoreCode, cancellationToken)
            .ConfigureAwait(false);
        
        _logger.LogInformation("Successfully sent restore code for {UserId} to email {Email}.", user.Id, user.Email);
        
        return Result.Success();
    }
}