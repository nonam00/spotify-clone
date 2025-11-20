using Microsoft.Extensions.Logging;

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
        var moderator = await _moderatorsRepository.GetById(command.ModeratorId, cancellationToken);

        if (moderator is null)
        {
            _logger.LogWarning("Moderator {ModeratorId} not found while updating permissions", command.ModeratorId);
            return Result.Failure(ModeratorErrors.NotFound);
        }

        var permissions = new ModeratorPermissions(
            command.CanManageUsers,
            command.CanManageContent,
            command.CanViewReports,
            command.CanManageModerators);

        moderator.UpdatePermissions(permissions);
        _moderatorsRepository.Update(moderator);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Moderator {ModeratorId} permissions were updated", command.ModeratorId);

        return Result.Success();
    }
}

