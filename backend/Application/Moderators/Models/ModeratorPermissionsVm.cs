namespace Application.Moderators.Models;

public sealed record ModeratorPermissionsVm(
    bool CanManageUsers,
    bool CanManageContent,
    bool CanViewReports,
    bool CanManageModerators);
    