using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetUnpublishedSongList;

public record GetUnpublishedSongListQuery : IQuery<Result<SongListVm>>;