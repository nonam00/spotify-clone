using Application.Shared.Messaging;

namespace Application.LikedSongs.Queries.CheckLikedSong;

public class CheckLikedSongQuery : IQuery<bool>
{
    public Guid UserId { get; init; }
    public Guid SongId { get; init; }
}