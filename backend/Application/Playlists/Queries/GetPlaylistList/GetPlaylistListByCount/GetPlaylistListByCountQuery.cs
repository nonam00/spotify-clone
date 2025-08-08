using MediatR;

using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistList.GetPlaylistListByCount;

public class GetPlaylistListByCountQuery : IRequest<PlaylistListVm>
{
    public Guid UserId { get; init; }
    public int Count { get; init; }
}