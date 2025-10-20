using Application.Shared.Messaging;

namespace Application.Playlists.Commands.DeletePlaylist;

public class DeletePlaylistCommand : ICommand<string?>
{
    public Guid PlaylistId { get; init; }
    public Guid UserId { get; init; }
}