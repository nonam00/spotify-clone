using MediatR;

namespace Application.Playlists.Commands.DeletePlaylist
{
    public class DeletePlaylistCommand : IRequest
    {
        public Guid UserId { get; init;}
        public Guid PlaylistId { get; init; }
    }
}
