using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetLikedSongListForPlaylist;

public record GetLikedSongListForPlaylistQuery(Guid UserId, Guid PlaylistId) : IQuery<Result<SongListVm>>;