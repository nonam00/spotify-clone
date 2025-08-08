namespace Application.Songs.Models;

public class SongVm
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string Author { get; init; } = null!;
    public string SongPath { get; init; } = null!;
    public string ImagePath { get; init; } = null!;
}