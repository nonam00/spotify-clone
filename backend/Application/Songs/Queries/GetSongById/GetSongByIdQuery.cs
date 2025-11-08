using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongById;

public record GetSongByIdQuery(Guid SongId) : IQuery<Result<SongVm>>;