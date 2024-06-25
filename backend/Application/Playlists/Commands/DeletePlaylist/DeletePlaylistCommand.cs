using MediatR;

namespace Application.Playlists.Commands.DeletePlaylist
{
    public class DeletePlaylistCommand : IRequest
    {
        public Guid UserId { get; set;}
        public Guid PlaylistId { get; set; }
    }
}
