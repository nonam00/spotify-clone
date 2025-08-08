using MediatR;

using Application.Songs.Enums;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongList.GetSongListBySearch;

public class GetSongListBySearchQuery : IRequest<SongListVm>
{
    public string SearchString { get; init; } = null!;
    public SearchCriteria SearchCriteria { get; init; }
}