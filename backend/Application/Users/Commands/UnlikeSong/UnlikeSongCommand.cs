using Application.Shared.Messaging;

namespace Application.Users.Commands.UnlikeSong;

public record UnlikeSongCommand(Guid UserId, Guid SongId) : ICommand;