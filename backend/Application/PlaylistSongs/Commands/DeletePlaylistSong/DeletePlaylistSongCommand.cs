using Application.Shared.Messaging;

namespace Application.PlaylistSongs.Commands.DeletePlaylistSong;

public record DeletePlaylistSongCommand(Guid UserId, Guid PlaylistId, Guid SongId) : ICommand;