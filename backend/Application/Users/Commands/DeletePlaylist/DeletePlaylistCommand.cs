using Application.Shared.Messaging;

namespace Application.Users.Commands.DeletePlaylist;

public record DeletePlaylistCommand(Guid PlaylistId, Guid UserId) : ICommand;