using Application.Shared.Messaging;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetLikedSongListForPlaylist;

public record GetLikedSongListForPlaylistQuery(Guid UserId, Guid PlaylistId) : IQuery<Result<SongListVm>>;