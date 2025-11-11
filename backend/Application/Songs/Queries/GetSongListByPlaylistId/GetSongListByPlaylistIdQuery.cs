using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongListByPlaylistId;

public record GetSongListByPlaylistIdQuery(Guid PlaylistId) : IQuery<Result<SongListVm>>;