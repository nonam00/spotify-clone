using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Models;
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
            _logger.LogWarning(
                "Managing moderator {ManagingModeratorId} not found while updating permissions for moderator {ModeratorToUpdateId}",
                command.ManagingModeratorId, command.ModeratorToUpdateId);
            return Result.Failure(ModeratorErrors.NotFound);
        }
        
        if (!managingModerator.IsActive)
        {
            _logger.LogInformation(
                "Tried to create moderator but managing moderator {ManagingModeratorId} does not exist.",
                command.ManagingModeratorId);
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!managingModerator.Permissions.CanManageModerators)
        {
            _logger.LogWarning(
                "Managing moderator {ManagingModeratorId} cannot manage moderators, so cannot update permissions for moderator {ModeratorToUpdateId}",
                command.ManagingModeratorId, command.ModeratorToUpdateId);
            return Result.Failure(ModeratorDomainErrors.CannotManageModerators);
        }
        
        var moderatorToUpdate = await _moderatorsRepository.GetById(command.ModeratorToUpdateId, cancellationToken);
        
        if (moderatorToUpdate is null)
        {
            _logger.LogWarning(
                "Moderator to update {ModeratorToUpdateId} not found while updating permissions",
                command.ModeratorToUpdateId);
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
            return Result.Failure(updateResult.Error);
        }
        
        _moderatorsRepository.Update(moderatorToUpdate);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Moderator {ModeratorToUpdateId} permissions were updated by managing moderator {ManagingModeratorId}",
            command.ModeratorToUpdateId, command.ManagingModeratorId);

        return Result.Success();
    }
}

