using MediatR;

namespace Application.Playlists.Queries.GetPlaylistById
{
    public class GetPlaylistByIdQuery : IRequest<PlaylistVm>
    {
        public Guid Id { get; set; }
    } 
}
