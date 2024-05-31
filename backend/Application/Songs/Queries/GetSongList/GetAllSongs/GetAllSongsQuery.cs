using MediatR;

namespace Application.Songs.Queries.GetSongList.GetAllSongs
{
    public class GetAllSongsQuery : IRequest<SongListVm>;
}
