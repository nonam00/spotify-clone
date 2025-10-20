using Application.Shared.Messaging;

using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistById;

public class GetPlaylistByIdQuery : IQuery<PlaylistVm>
{
    public Guid PlaylistId { get; init; }
    public Guid UserId { get; set; }
}