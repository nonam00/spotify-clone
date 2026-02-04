using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Playlists.Commands.UpdatePlaylist;

public record UpdatePlaylistCommand(
    Guid UserId, Guid PlaylistId, string Title, string? Description, string? ImagePath): ICommand<Result>;