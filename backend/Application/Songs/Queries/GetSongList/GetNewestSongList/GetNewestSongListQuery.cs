using MediatR;

namespace Application.Songs.Queries.GetSongList.GetNewestSongList
{
    public class GetNewestSongListQuery : IRequest<SongListVm>;
}
