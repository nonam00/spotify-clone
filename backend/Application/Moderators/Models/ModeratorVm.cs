namespace Application.Moderators.Models;

public record ModeratorVm(
    Guid Id,
    string Email,
    string FullName,
    bool IsActive,
    DateTime CreatedAt,
    ModeratorPermissionsVm Permissions);
