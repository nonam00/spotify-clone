using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.LikeSong;

public record LikeSongCommand(Guid UserId, Guid SongId) : ICommand<Result>;
