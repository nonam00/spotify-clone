namespace Application.Moderators.Models;

public record ModeratorInfo(
    Guid Id,
    string Email,
    string FullName,
    bool IsActive,
    ModeratorPermissionsVm Permissions);
    