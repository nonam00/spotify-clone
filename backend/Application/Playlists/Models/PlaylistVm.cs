namespace Application.Playlists.Models;

public class PlaylistVm
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public string? ImagePath { get; init; }
}
