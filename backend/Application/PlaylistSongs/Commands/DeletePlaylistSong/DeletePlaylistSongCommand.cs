using MediatR;

namespace Application.PlaylistSongs.Commands.DeletePlaylistSong
{
    public class DeletePlaylistSongCommand : IRequest
    {
        public Guid PlaylistId { get; set; }
        public Guid SongId { get; set; }
    }
}
