using Application.Songs.Enums;
using MediatR;

namespace Application.Songs.Queries.GetSongList.GetSongListBySearch;

public class GetSongListBySearchQuery : IRequest<SongListVm>
{
    public string SearchString { get; set; }
    public SearchCriteria SearchCriteria { get; set; }
}