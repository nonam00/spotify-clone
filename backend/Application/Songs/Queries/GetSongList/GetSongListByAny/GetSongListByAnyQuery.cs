using MediatR;

namespace Application.Songs.Queries.GetSongList.GetSongListByAny
{
    public class GetSongListByAnyQuery : IRequest<SongListVm>
    {
        public string SearchString { get; init; } = null!;
    }
}
