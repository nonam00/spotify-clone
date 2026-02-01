using Application.Shared.Messaging;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetSongById;

public record GetSongByIdQuery(Guid SongId) : IQuery<Result<SongVm>>;