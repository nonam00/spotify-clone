using MediatR;

namespace Application.LikedSongs.Queries.CheckLikedSong
{
    public class CheckLikedSongQuery : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public Guid SongId { get; set; }
    }
}
