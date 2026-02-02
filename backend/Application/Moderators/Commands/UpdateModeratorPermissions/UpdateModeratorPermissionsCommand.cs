using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.UpdateModeratorPermissions;

public record UpdateModeratorPermissionsCommand(
    Guid ManagingModeratorId, Guid ModeratorToUpdatePermissionsId,
    bool CanManageUsers, bool CanManageContent, bool CanViewReports, bool CanManageModerators) : ICommand<Result>;