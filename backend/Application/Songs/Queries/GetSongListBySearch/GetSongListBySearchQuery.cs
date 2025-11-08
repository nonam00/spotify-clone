using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Enums;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongListBySearch;

public record GetSongListBySearchQuery(string SearchString, SearchCriteria SearchCriteria)
    : IQuery<Result<SongListVm>>;