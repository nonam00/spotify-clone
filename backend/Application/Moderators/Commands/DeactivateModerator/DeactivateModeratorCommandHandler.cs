using Microsoft.Extensions.Logging;
using Domain.Common;

using Domain.Models;
using Application.Moderators.Commands.ActivateModerator;
using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.DeactivateModerator;

public class DeactivateModeratorCommandHandler : ICommandHandler<DeactivateModeratorCommand, Result>
{
    private readonly IModeratorsRepository _moderatorsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateModeratorCommandHandler> _logger;

    public DeactivateModeratorCommandHandler(
        IModeratorsRepository moderatorsRepository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateModeratorCommandHandler> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _moderatorsRepository = moderatorsRepository;
    }

    public async Task<Result> Handle(DeactivateModeratorCommand command, CancellationToken cancellationToken)
    {
        var managingModerator = await _moderatorsRepository.GetById(command.ManagingModeratorId, cancellationToken);

        if (managingModerator == null)
        {
            _logger.LogInformation(
                "Tried to deactivate moderator {ModeratorToActivateId}" +
                " but managing moderator {ManagingModeratorId} does not exist.",
                command.ModeratorToDeactivateId, command.ManagingModeratorId);
            return Result.Failure(ModeratorErrors.NotFound);
        }

        if (!managingModerator.IsActive)
        {
            _logger.LogInformation(
                "Tried to deactivate moderator {ModeratorToActivateId}" +
                " but managing moderator {ManagingModeratorId} does not exist.",
                command.ModeratorToDeactivateId, command.ManagingModeratorId);
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }
        
        if (!managingModerator.Permissions.CanManageModerators)
        {
            _logger.LogWarning(
                "Tried to deactivate moderator {ModeratorToActivateId}" +
                " but managing moderator {ManagingModeratorId} cannot manage moderators",
                command.ModeratorToDeactivateId, command.ManagingModeratorId);
            return Result.Failure(ModeratorDomainErrors.CannotManageModerators);
        }

        var moderatorToDeactivate = await _moderatorsRepository.GetById(command.ModeratorToDeactivateId, cancellationToken);
        
        if (moderatorToDeactivate == null)
        {
            _logger.LogWarning(
                "Tried to deactivate moderator {ModeratorToActivateId} but it does not exist.",
                command.ModeratorToDeactivateId);
            return Result.Failure(ModeratorErrors.NotFound);
        }
        
        var deactivationResult = managingModerator.DeactivateModerator(moderatorToDeactivate);

        if (deactivationResult.IsFailure)
        {
            return deactivationResult;
        }
        
        _moderatorsRepository.Update(moderatorToDeactivate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}