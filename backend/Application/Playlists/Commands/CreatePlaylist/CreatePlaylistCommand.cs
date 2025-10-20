using Application.Shared.Messaging;

namespace Application.Playlists.Commands.CreatePlaylist;

public class CreatePlaylistCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
}