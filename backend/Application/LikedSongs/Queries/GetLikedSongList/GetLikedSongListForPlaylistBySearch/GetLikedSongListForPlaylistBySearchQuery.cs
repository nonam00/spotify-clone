using Application.LikedSongs.Models;
using Application.Shared.Messaging;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListForPlaylistBySearch;

public record GetLikedSongListForPlaylistBySearchQuery(
    Guid UserId, Guid PlaylistId, string SearchString): IQuery<LikedSongListVm>;