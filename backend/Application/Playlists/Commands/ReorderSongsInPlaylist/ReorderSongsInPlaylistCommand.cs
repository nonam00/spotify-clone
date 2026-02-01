using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Playlists.Commands.ReorderSongsInPlaylist;

public record ReorderSongsInPlaylistCommand(Guid UserId, Guid PlaylistId, List<Guid> SongIds) : ICommand<Result>;