using Application.Shared.Messaging;

namespace Application.Playlists.Commands.DeletePlaylist;

public record DeletePlaylistCommand(Guid PlaylistId, Guid UserId) : ICommand<string?>;