using Application.Shared.Messaging;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetUnpublishedSongList;

public record GetUnpublishedSongListQuery : IQuery<Result<SongListVm>>;