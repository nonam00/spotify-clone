using Application.LikedSongs.Models;
using Application.Shared.Messaging;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListForPlaylist;

public class GetLikedSongListForPlaylistQuery : IQuery<LikedSongListVm>
{
    public Guid UserId { get; init; }
    public Guid PlaylistId { get; init; }
}