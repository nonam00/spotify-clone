using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Errors;
using Domain.ValueObjects;
using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.UpdateModeratorPermissions;

public class UpdateModeratorPermissionsCommandHandler : ICommandHandler<UpdateModeratorPermissionsCommand, Result>
{
    private readonly IModeratorsRepository _moderatorsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateModeratorPermissionsCommandHandler> _logger;

    public UpdateModeratorPermissionsCommandHandler(
        IModeratorsRepository moderatorsRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateModeratorPermissionsCommandHandler> logger)
    {
        _moderatorsRepository = moderatorsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateModeratorPermissionsCommand command, CancellationToken cancellationToken)
    {
        var managingModerator = await _moderatorsRepository.GetById(command.ManagingModeratorId, cancellationToken);

        if (managingModerator is null)
        {
            _logger.LogError(
                "Tried to update permission for moderator {ModeratorToUpdatePermissionsId}" +
                " but managing moderator {ManagingModeratorId} doesnt exist.",
                command.ModeratorToUpdatePermissionsId, command.ManagingModeratorId);
            return Result.Failure(ModeratorErrors.NotFound);
        }
        
        if (!managingModerator.IsActive)
        {
            _logger.LogError(
                "Tried to update permissions for moderator {ModeratorToUpdatePermissionsId}" +
                " but managing moderator {ManagingModeratorId} is not active.",
                command.ModeratorToUpdatePermissionsId, command.ManagingModeratorId);
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!managingModerator.Permissions.CanManageModerators)
        {
            _logger.LogWarning(
                "Managing moderator {ManagingModeratorId} tried" +
                " to update permissions for moderator {ModeratorToUpdatePermissionsId}" +
                " but doesnt have permission to manage moderators.",
                command.ManagingModeratorId, command.ModeratorToUpdatePermissionsId);
            return Result.Failure(ModeratorDomainErrors.CannotManageModerators);
        }
        
        var moderatorToUpdate = await _moderatorsRepository.GetById(command.ModeratorToUpdatePermissionsId, cancellationToken);
        
        if (moderatorToUpdate is null)
        {
            _logger.LogError(
                "Managing moderator {ManagingModeratorId} tried" +
                " to update permissions for moderator {ModeratorToUpdatePermissionsId}" +
                " but it doesnt exist.",
                command.ManagingModeratorId, command.ModeratorToUpdatePermissionsId);
            return Result.Failure(ModeratorErrors.NotFound);
        }

        var permissions = new ModeratorPermissions(
            command.CanManageUsers,
            command.CanManageContent,
            command.CanViewReports,
            command.CanManageModerators);

        var updateResult = managingModerator.UpdateModeratorPermissions(moderatorToUpdate, permissions);

        if (updateResult.IsFailure)
        {
            _logger.LogError(
                "Managing moderator {ManagingModeratorId} tried" +
                " to update permissions for moderator {ModeratorToUpdatePermissionsId}" +
                " but domain error occurred:\n{DomainErrorDescription}",
                command.ManagingModeratorId, command.ModeratorToUpdatePermissionsId, updateResult.Error.Description);
            return Result.Failure(updateResult.Error);
        }
        
        _moderatorsRepository.Update(moderatorToUpdate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Moderator {ModeratorToUpdateId} permissions were updated by managing moderator {ManagingModeratorId}.",
            command.ModeratorToUpdatePermissionsId, command.ManagingModeratorId);

        return Result.Success();
    }
}