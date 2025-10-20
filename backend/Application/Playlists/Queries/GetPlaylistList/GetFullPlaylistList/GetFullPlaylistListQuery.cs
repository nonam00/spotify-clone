using Application.Shared.Messaging;

using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistList.GetFullPlaylistList;

public class GetFullPlaylistListQuery : IQuery<PlaylistListVm>
{
    public Guid UserId { get; init; }
}