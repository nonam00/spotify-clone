using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Errors;
using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Moderators.Commands.ActivateUser;

public sealed class ActivateUserCommandHandler : ICommandHandler<ActivateUserCommand, Result>
{
    private readonly IModeratorsRepository _moderatorsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateUserCommandHandler> _logger;

    public ActivateUserCommandHandler(
        IModeratorsRepository moderatorsRepository,
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateUserCommandHandler> logger)
    {
        _moderatorsRepository = moderatorsRepository;
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ActivateUserCommand command, CancellationToken cancellationToken)
    {
        var moderator = await _moderatorsRepository
            .GetById(command.ModeratorId, cancellationToken)
            .ConfigureAwait(false);

        if (moderator is null)
        {
            _logger.LogError(
                "Tried to activate user {UserId} but moderator {ModeratorId} doesnt exist.",
                command.UserId, command.ModeratorId);
            return Result.Failure(ModeratorErrors.NotFound);
        }

        if (!moderator.IsActive)
        {
            _logger.LogError(
                "Moderator {ModeratorId} tried to activate user {UserId} but moderator is not active.",
                command.ModeratorId, command.UserId);
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!moderator.Permissions.CanManageUsers)
        {
            _logger.LogError(
                "Moderator {ModeratorId} tried to activate user {UserId}" +
                " but moderator doesnt have permission to manage users.",
                command.ModeratorId, command.UserId);
            return Result.Failure(ModeratorDomainErrors.CannotManageUsers);
        }
        
        var user = await _usersRepository.GetById(command.UserId, cancellationToken).ConfigureAwait(false);
        
        if (user is null)
        {
            _logger.LogError(
                "Moderator {ModeratorId} tried to activate user {UserId} but user doesnt exist.",
                command.ModeratorId, command.UserId);
            return Result.Failure(UserErrors.NotFound);
        }
        
        var activateUserResult = moderator.ActivateUser(user);
        if (activateUserResult.IsFailure)
        {
            _logger.LogError(
                "Moderator {ModeratorId} tried to activate user {UserId}" +
                " but domain error occurred:\n{DomainErrorDescription}",
                command.ModeratorId, command.UserId, activateUserResult.Error.Description);
            return activateUserResult;
        }
        
        _usersRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        
        Log.LogModeratorSuccessfullyActivatedUser(_logger, command.ModeratorId, command.UserId);

        return Result.Success();
    }
}

internal static partial class Log
{
    [LoggerMessage(
        LogLevel.Trace,
        "Moderator {ModeratorId} successfully activated user {UserId}.")]
    internal static partial void LogModeratorSuccessfullyActivatedUser(
        ILogger logger, Guid moderatorId, Guid userId);
}