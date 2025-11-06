using Application.Shared.Messaging;

namespace Application.Playlists.Commands.RemoveSongFromPlaylist;

public record RemoveSongFromPlaylistCommand(Guid UserId, Guid PlaylistId, Guid SongId) : ICommand;