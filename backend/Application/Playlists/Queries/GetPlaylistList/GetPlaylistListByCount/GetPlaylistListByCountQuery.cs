using MediatR;

namespace Application.Playlists.Queries.GetPlaylistList.GetPlaylistListByCount
{
    public class GetPlaylistListByCountQuery : IRequest<PlaylistListVm>
    {
        public Guid UserId { get; set; }
        public int Count { get; set; }
    }
}
