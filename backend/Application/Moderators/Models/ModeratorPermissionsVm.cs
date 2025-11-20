namespace Application.Moderators.Models;

public record ModeratorPermissionsVm(
    bool CanManageUsers,
    bool CanManageContent,
    bool CanViewReports,
    bool CanManageModerators);
    