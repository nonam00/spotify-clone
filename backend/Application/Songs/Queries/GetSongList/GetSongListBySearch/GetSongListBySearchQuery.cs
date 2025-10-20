using Application.Shared.Messaging;

using Application.Songs.Enums;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongList.GetSongListBySearch;

public class GetSongListBySearchQuery : IQuery<SongListVm>
{
    public string SearchString { get; init; } = null!;
    public SearchCriteria SearchCriteria { get; init; }
}