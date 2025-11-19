using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Playlists.Commands.ReorderSongsInPlaylist;

public record ReorderSongsInPlaylistCommand(Guid PlaylistId, List<Guid> SongIds) : ICommand<Result>;