using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.CreatePlaylist;

public record CreatePlaylistCommand(Guid UserId) : ICommand<Result<Guid>>;