namespace Domain.ValueObjects;

// Value object for moderator rights
public class ModeratorPermissions
{
    public bool CanManageUsers { get; private set; }
    public bool CanManageContent { get; private set; }
    public bool CanViewReports { get; private set; }
    public bool CanManageModerators { get; private set; }

    public ModeratorPermissions(bool canManageUsers, bool canManageContent, bool canViewReports, bool canManageModerators)
    {
        CanManageUsers = canManageUsers;
        CanManageContent = canManageContent;
        CanViewReports = canViewReports;
        CanManageModerators = canManageModerators;
    }

    public static ModeratorPermissions CreateDefault()
    {
        return new ModeratorPermissions(
            canManageContent: true,
            canManageUsers: true,
            canViewReports: true,
            canManageModerators: false);
    }

    public static ModeratorPermissions CreateSuperAdmin()
    {
        return new ModeratorPermissions(
            canManageContent: true,
            canManageUsers: true,
            canViewReports: true,
            canManageModerators: true);
    }
}