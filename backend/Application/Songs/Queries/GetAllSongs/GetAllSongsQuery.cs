using Application.Shared.Messaging;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetAllSongs;

public record GetAllSongsQuery : IQuery<Result<SongListVm>>;