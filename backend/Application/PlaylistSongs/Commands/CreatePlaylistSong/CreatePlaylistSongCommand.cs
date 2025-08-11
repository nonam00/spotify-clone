using MediatR;

namespace Application.PlaylistSongs.Commands.CreatePlaylistSong;

public class CreatePlaylistSongCommand : IRequest<string>
{
    public Guid UserId { get; init; }
    public Guid PlaylistId { get; init; }
    public Guid SongId { get; init; }
}