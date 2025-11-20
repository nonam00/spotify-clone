using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.UpdateModeratorPermissions;

public record UpdateModeratorPermissionsCommand(
    Guid ModeratorId,
    bool CanManageUsers,
    bool CanManageContent,
    bool CanViewReports,
    bool CanManageModerators) : ICommand<Result>;

