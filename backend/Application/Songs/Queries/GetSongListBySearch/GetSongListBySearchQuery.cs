using Application.Shared.Messaging;
using Application.Songs.Enums;
using Application.Songs.Models;
using Domain.Common;

namespace Application.Songs.Queries.GetSongListBySearch;

public record GetSongListBySearchQuery(string SearchString, SearchCriteria SearchCriteria)
    : IQuery<Result<SongListVm>>;