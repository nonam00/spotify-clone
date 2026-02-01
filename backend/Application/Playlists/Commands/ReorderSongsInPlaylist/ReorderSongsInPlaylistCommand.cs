using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Playlists.Commands.ReorderSongsInPlaylist;

public record ReorderSongsInPlaylistCommand(Guid PlaylistId, List<Guid> SongIds) : ICommand<Result>;