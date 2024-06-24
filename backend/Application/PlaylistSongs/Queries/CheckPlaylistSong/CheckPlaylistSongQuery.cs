using MediatR;

namespace Application.PlaylistSongs.Queries.CheckPlaylistSong
{
    public class CheckPlaylistSongQuery : IRequest<bool>
    {
        public Guid PlaylistId { get; set; }
        public Guid SongId { get; set; }
    }
}
