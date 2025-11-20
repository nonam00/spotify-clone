namespace Application.Users.Models;

public record UserVm(
    Guid Id,
    string Email,
    string FullName,
    bool IsActive,
    DateTime CreatedAt,
    int UploadedSongsCount);

