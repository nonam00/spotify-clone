using MediatR;

namespace Application.PlaylistSongs.Commands.DeletePlaylistSong;

public class DeletePlaylistSongCommand : IRequest
{
    public Guid UserId { get; init; }
    public Guid PlaylistId { get; init; }
    public Guid SongId { get; init; }
}