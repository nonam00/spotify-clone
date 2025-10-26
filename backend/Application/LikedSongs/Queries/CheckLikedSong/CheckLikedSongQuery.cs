using Application.Shared.Messaging;

namespace Application.LikedSongs.Queries.CheckLikedSong;

public record CheckLikedSongQuery(Guid UserId, Guid SongId) : IQuery<bool>;