using Microsoft.Extensions.Logging;

using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.UpdateModeratorStatus;

public class UpdateModeratorStatusCommandHandler : ICommandHandler<UpdateModeratorStatusCommand, Result>
{
    private readonly IModeratorsRepository _moderatorsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateModeratorStatusCommandHandler> _logger;

    public UpdateModeratorStatusCommandHandler(
        IModeratorsRepository moderatorsRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateModeratorStatusCommandHandler> logger)
    {
        _moderatorsRepository = moderatorsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateModeratorStatusCommand command, CancellationToken cancellationToken)
    {
        var moderator = await _moderatorsRepository.GetById(command.ModeratorId, cancellationToken);

        if (moderator is null)
        {
            _logger.LogWarning("Moderator {ModeratorId} not found while updating status", command.ModeratorId);
            return Result.Failure(ModeratorErrors.NotFound);
        }

        if (command.IsActive)
        {
            moderator.Activate();
        }
        else
        {
            moderator.Deactivate();
        }

        _moderatorsRepository.Update(moderator);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Moderator {ModeratorId} status changed to {Status}", command.ModeratorId, command.IsActive);

        return Result.Success();
    }
}

