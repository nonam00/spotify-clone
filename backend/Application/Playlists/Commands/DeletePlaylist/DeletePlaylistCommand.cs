using MediatR;

namespace Application.Playlists.Commands.DeletePlaylist
{
    public class DeletePlaylistCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
