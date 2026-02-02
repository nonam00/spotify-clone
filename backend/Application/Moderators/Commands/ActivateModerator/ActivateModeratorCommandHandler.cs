using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Models;
using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.ActivateModerator;

public class ActivateModeratorCommandHandler : ICommandHandler<ActivateModeratorCommand, Result>
{
    private readonly IModeratorsRepository _moderatorsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateModeratorCommandHandler> _logger;

    public ActivateModeratorCommandHandler(
        IModeratorsRepository moderatorsRepository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateModeratorCommandHandler> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _moderatorsRepository = moderatorsRepository;
    }

    public async Task<Result> Handle(ActivateModeratorCommand command, CancellationToken cancellationToken)
    {
        var managingModerator = await _moderatorsRepository.GetById(command.ManagingModeratorId, cancellationToken);

        if (managingModerator is null)
        {
            _logger.LogError(
                "Tried to activate moderator {ModeratorToActivateId}" +
                " but managing moderator {ManagingModeratorId} does not exist.",
                command.ModeratorToActivateId, command.ManagingModeratorId);
            return Result.Failure(ModeratorErrors.NotFound);
        }

        if (!managingModerator.IsActive)
        {
            _logger.LogError(
                "Tried to activate moderator {ModeratorToActivateId}" +
                " but managing moderator {ManagingModeratorId} does not exist.",
                command.ModeratorToActivateId, command.ManagingModeratorId);
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }
        
        if (!managingModerator.Permissions.CanManageModerators)
        {
            _logger.LogWarning(
                "Tried to activate moderator {ModeratorToActivateId}" +
                " but managing moderator {ManagingModeratorId} cannot manage moderators.",
                command.ModeratorToActivateId, command.ManagingModeratorId);
            return Result.Failure(ModeratorDomainErrors.CannotManageModerators);
        }

        var moderatorToActivate = await _moderatorsRepository.GetById(command.ModeratorToActivateId, cancellationToken);
        
        if (moderatorToActivate is null)
        {
            _logger.LogError(
                "Managing moderator {ManagingModeratorId} tried to activate moderator {ModeratorToActivateId}" +
                " but it does not exist.",
                command.ManagingModeratorId, command.ModeratorToActivateId);
            return Result.Failure(ModeratorErrors.NotFound);
        }
        
        var activationResult = managingModerator.ActivateModerator(moderatorToActivate);

        if (activationResult.IsFailure)
        {
            _logger.LogError(
                "Managing moderator {ManagingModeratorId} tried to activate moderator {ModeratorToActivateId}" +
                " but domain error occurred: {DomainErrorDescription}.",
                command.ManagingModeratorId, command.ModeratorToActivateId, activationResult.Error.Description);
            return activationResult;
        }
        
        _moderatorsRepository.Update(moderatorToActivate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "Moderator {ModeratorToActivateId} was successfully activated by managing moderator {ManagingModeratorId}.",
            command.ModeratorToActivateId, command.ManagingModeratorId);
        
        return Result.Success();
    }
}