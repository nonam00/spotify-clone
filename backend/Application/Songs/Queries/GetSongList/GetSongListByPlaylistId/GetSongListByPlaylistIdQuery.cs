using MediatR;

namespace Application.Songs.Queries.GetSongList.GetSongListByPlaylistId
{
    public class GetSongListByPlaylistIdQuery : IRequest<SongListVm>
    {
        public Guid PlaylistId { get; init; } 
    }
}
