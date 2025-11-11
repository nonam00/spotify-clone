using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetNewestSongList;

public class GetNewestSongListQuery : IQuery<Result<SongListVm>>;