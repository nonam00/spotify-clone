using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.UnlikeSong;

public record UnlikeSongCommand(Guid UserId, Guid SongId) : ICommand<Result>;