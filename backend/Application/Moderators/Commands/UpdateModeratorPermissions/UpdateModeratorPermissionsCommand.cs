using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Moderators.Commands.UpdateModeratorPermissions;

public record UpdateModeratorPermissionsCommand(
    Guid ManagingModeratorId,
    Guid ModeratorToUpdateId,
    bool CanManageUsers,
    bool CanManageContent,
    bool CanViewReports,
    bool CanManageModerators) : ICommand<Result>;

