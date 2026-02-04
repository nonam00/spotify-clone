using Application.Shared.Messaging;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetNewestSongList;

public class GetNewestSongListQuery : IQuery<Result<SongListVm>>;