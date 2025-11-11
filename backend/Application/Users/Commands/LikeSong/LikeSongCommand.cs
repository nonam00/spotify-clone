using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Users.Commands.LikeSong;

public record LikeSongCommand(Guid UserId, Guid SongId) : ICommand<Result>;
