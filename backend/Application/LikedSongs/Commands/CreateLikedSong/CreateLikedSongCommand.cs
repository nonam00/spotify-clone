using MediatR;

namespace Application.LikedSongs.Commands.CreateLikedSong
{
    public class CreateLikedSongCommand : IRequest<string>
    {
        public Guid UserId { get; init; }
        public Guid SongId { get; init; }
    }
}
