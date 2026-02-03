using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Errors;
using Application.Moderators.Commands.ActivateUser;
using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Moderators.Commands.DeactivateUser;

public class DeactivateUserCommandHandler : ICommandHandler<DeactivateUserCommand, Result>
{
    private readonly IModeratorsRepository _moderatorsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateUserCommandHandler> _logger;

    public DeactivateUserCommandHandler(
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

    public async Task<Result> Handle(DeactivateUserCommand command, CancellationToken cancellationToken)
    {
        var moderator = await _moderatorsRepository.GetById(command.ModeratorId, cancellationToken);

        if (moderator is null)
        {
            _logger.LogError(
                "Tried to deactivate user {UserId} but moderator {ModeratorId} doesnt exist.",
                command.UserId, command.ModeratorId);
            return Result.Failure(ModeratorErrors.NotFound);
        }

        if (!moderator.IsActive)
        {
            _logger.LogError(
                "Moderator {ModeratorId} tried to deactivate user {UserId} but moderator is not active.",
                command.ModeratorId, command.UserId);
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!moderator.Permissions.CanManageUsers)
        {
            _logger.LogError(
                "Moderator {ModeratorId} tried to deactivate user {UserId}" +
                " but moderator doesnt have permission to manage users.",
                command.ModeratorId, command.UserId);
            return Result.Failure(ModeratorDomainErrors.CannotManageUsers);
        }
        
        var user = await _usersRepository.GetById(command.UserId, cancellationToken);
        
        if (user is null)
        {
            _logger.LogError(
                "Moderator {ModeratorId} tried to deactivate user {UserId} but user doesnt exist.",
                command.ModeratorId, command.UserId);
            return Result.Failure(UserErrors.NotFound);
        }
        
        var activateUserResult = moderator.DeactivateUser(user);
        if (activateUserResult.IsFailure)
        {
            _logger.LogError(
                "Moderator {ModeratorId} tried to deactivate user {UserId}" +
                " but domain error occurred:\n{DomainErrorDescription}",
                command.ModeratorId, command.UserId, activateUserResult.Error.Description);
            return activateUserResult;
        }
        
        _usersRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "Moderator {ModeratorId} successfully deactivated User {UserId}.",
            command.ModeratorId, command.UserId);

        return Result.Success();
    }
}