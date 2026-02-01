using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.DeletePlaylist;

public record DeletePlaylistCommand(Guid PlaylistId, Guid UserId) : ICommand<Result>;