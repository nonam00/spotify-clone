using MediatR;

namespace Application.Songs.Queries.GetSongList.GetSongListByTitle
{
    public class GetSongListByTitleQuery : IRequest<SongListVm>
    {
        public string SearchString { get; init; } = null!;
    }
}
