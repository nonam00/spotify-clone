using MediatR;

namespace Application.LikedSongs.Queries.CheckLikedSong;

public class CheckLikedSongQuery : IRequest<bool>
{
    public Guid UserId { get; init; }
    public Guid SongId { get; init; }
}