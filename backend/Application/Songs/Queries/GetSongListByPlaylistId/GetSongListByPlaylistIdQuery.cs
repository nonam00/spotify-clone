using Application.Shared.Messaging;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetSongListByPlaylistId;

public record GetSongListByPlaylistIdQuery(Guid PlaylistId) : IQuery<Result<SongListVm>>;