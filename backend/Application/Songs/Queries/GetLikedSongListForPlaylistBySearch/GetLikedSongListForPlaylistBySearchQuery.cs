using Application.Shared.Messaging;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetLikedSongListForPlaylistBySearch;

public record GetLikedSongListForPlaylistBySearchQuery(
    Guid UserId, Guid PlaylistId, string SearchString): IQuery<SongListVm>;