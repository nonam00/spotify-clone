using Application.Shared.Messaging;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetLikedSongListForPlaylistBySearch;

public record GetLikedSongListForPlaylistBySearchQuery(
    Guid UserId, Guid PlaylistId, string SearchString): IQuery<Result<SongListVm>>;