using Application.Shared.Messaging;

namespace Application.Users.Commands.CreatePlaylist;

public record CreatePlaylistCommand(Guid UserId) : ICommand<Guid>;