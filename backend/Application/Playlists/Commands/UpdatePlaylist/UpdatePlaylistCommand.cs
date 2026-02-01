using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Playlists.Commands.UpdatePlaylist;

public record UpdatePlaylistCommand(
    Guid UserId, Guid PlaylistId, string Title, string? Description, string? ImagePath): ICommand<Result>;