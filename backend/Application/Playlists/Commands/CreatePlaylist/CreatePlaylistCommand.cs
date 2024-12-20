using MediatR;

namespace Application.Playlists.Commands.CreatePlaylist
{
    public class CreatePlaylistCommand : IRequest<Guid>
    {
        public Guid UserId { get; init; }
    }
}
