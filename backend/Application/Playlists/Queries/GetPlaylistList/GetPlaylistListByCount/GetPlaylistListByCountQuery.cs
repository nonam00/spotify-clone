using Application.Shared.Messaging;

using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistList.GetPlaylistListByCount;

public class GetPlaylistListByCountQuery : IQuery<PlaylistListVm>
{
    public Guid UserId { get; init; }
    public int Count { get; init; }
}