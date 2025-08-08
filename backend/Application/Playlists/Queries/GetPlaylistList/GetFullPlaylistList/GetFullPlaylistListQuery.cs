using MediatR;

using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistList.GetFullPlaylistList;

public class GetFullPlaylistListQuery : IRequest<PlaylistListVm>
{
    public Guid UserId { get; init; }
}