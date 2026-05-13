namespace Application.Moderators.Models;

public sealed record ModeratorVm(
    Guid Id,
    string Email,
    string FullName,
    bool IsActive,
    DateTime CreatedAt,
    ModeratorPermissionsVm Permissions);
