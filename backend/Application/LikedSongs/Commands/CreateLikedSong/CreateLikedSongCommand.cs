using MediatR;

namespace Application.LikedSongs.Commands.CreateLikedSong
{
    public class CreateLikedSongCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public Guid SongId { get; set; }
    }
}
