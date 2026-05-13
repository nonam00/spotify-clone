namespace Application.Moderators.Models;

public sealed record ModeratorInfo(
    Guid Id,
    string Email,
    string FullName,
    bool IsActive,
    ModeratorPermissionsVm Permissions);
    