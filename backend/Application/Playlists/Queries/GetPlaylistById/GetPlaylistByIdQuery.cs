using MediatR;

using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistById;

public class GetPlaylistByIdQuery : IRequest<PlaylistVm>
{
    public Guid PlaylistId { get; init; }
    public Guid UserId { get; set; }
}