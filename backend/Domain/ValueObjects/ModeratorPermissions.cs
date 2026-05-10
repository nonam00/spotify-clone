namespace Domain.ValueObjects;

// Value object for moderator rights
public sealed record ModeratorPermissions(
    bool CanManageUsers,
    bool CanManageContent,
    bool CanViewReports,
    bool CanManageModerators)
{
    public static ModeratorPermissions CreateDefault()
    {
        return new ModeratorPermissions(
            CanManageContent: true,
            CanManageUsers: true,
            CanViewReports: true,
            CanManageModerators: false);
    }

    public static ModeratorPermissions CreateSuperAdmin()
    {
        return new ModeratorPermissions(
            CanManageContent: true,
            CanManageUsers: true,
            CanViewReports: true,
            CanManageModerators: true);
    }
}