using MediatR;

namespace Application.Playlists.Queries.GetPlaylistListByUserId
{
    public class GetPlaylistListByUserIdQuery : IRequest<PlaylistListVm>
    {
        public Guid UserId { get; set; }
    } 
}
