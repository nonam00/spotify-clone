namespace Infrastructure.Auth;

public static class CustomClaims
{
    internal const string UserId = "userId";
    public const string EntityType = "entityType";
    internal const string ModeratorId = "moderatorId";
    public const string Permission = "permission";
}

public static class EntityTypes
{
    public const string User = "user";
    public const string Moderator = "moderator";
}

public static class Permissions
{
    public const string ManageUsers = "manage_users";
    public const string ManageContent = "manage_content";
    public const string ViewReports = "view_reports";
    public const string ManageModerators = "manage_moderators";
}