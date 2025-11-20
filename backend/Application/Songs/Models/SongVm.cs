namespace Application.Songs.Models;

public record SongVm(
    Guid Id,
    string Title,
    string Author,
    string? SongPath,
    string? ImagePath,
    bool IsPublished,
    DateTime CreatedAt);