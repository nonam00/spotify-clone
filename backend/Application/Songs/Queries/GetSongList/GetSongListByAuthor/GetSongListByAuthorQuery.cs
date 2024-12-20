using MediatR;

namespace Application.Songs.Queries.GetSongList.GetSongListByAuthor
{
    public class GetSongListByAuthorQuery : IRequest<SongListVm>
    {
        public string SearchString { get; init; } = null!;
    }
}
