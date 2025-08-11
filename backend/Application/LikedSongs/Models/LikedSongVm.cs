namespace Application.LikedSongs.Models;

public class LikedSongVm
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string Author { get; init; } = null!;
    public string SongPath { get; init; } = null!;
    public string ImagePath { get; init; } = null!;
}
