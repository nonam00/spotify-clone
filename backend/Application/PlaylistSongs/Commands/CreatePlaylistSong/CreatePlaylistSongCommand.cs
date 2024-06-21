using MediatR;

namespace Application.PlaylistSongs.Commands.CreatePlaylistSong
{
    public class CreatePlaylistSongCommand : IRequest<string>
    {
        public Guid PlaylistId { get; set; }
        public Guid SongId { get; set; }
    }
}
