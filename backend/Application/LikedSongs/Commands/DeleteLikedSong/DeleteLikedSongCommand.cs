using MediatR;

namespace Application.LikedSongs.Commands.DeleteLikedSong
{
    public class DeleteLikedSongCommand : IRequest
    {
        public Guid UserId { get; set; }
        public Guid SongId { get; set; }
    }
}
