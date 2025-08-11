using MediatR;

namespace Application.Songs.Commands.CreateSong;

public class CreateSongCommand : IRequest<Guid>
{
    public Guid UserId { get; init; }
    public string Title { get; init; } = null!;
    public string Author { get; init; } = null!;
    public string SongPath { get; init; } = null!;
    public string ImagePath { get; init; } = null!;
}