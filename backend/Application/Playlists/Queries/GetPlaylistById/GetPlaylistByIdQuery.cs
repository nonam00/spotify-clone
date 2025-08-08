using MediatR;

using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistById;

public class GetPlaylistByIdQuery : IRequest<PlaylistVm>
{
    public Guid Id { get; init; }
}