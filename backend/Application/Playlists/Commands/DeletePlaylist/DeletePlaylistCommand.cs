using MediatR;

namespace Application.Playlists.Commands.DeletePlaylist;

public class DeletePlaylistCommand : IRequest<string?>
{
    public Guid PlaylistId { get; init; }
    public Guid UserId { get; init; }
}