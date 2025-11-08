using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetAllSongs;

public record GetAllSongsQuery : IQuery<Result<SongListVm>>;