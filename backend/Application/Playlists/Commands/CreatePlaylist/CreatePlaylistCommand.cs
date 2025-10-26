using Application.Shared.Messaging;

namespace Application.Playlists.Commands.CreatePlaylist;

public record CreatePlaylistCommand(Guid UserId) : ICommand<Guid>;