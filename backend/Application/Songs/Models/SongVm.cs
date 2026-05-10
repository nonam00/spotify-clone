namespace Application.Songs.Models;

public record SongVm(
    Guid Id,
    string Title,
    string Author,
    string? AudioPath,
    string? ImagePath,
    bool ContainsExplicitContent,
    bool IsPublished,
    DateTime CreatedAt);