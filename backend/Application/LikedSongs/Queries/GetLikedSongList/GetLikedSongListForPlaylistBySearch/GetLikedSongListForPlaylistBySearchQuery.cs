using Application.LikedSongs.Models;
using Application.Shared.Messaging;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListForPlaylistBySearch;

public class GetLikedSongListForPlaylistBySearchQuery : IQuery<LikedSongListVm>
{
    public Guid UserId { get; init; }
    public Guid PlaylistId { get; init; }
    public string SearchString { get; init; } = null!;
}