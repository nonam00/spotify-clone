using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Playlists.Commands.RemoveSongFromPlaylist;

public record RemoveSongFromPlaylistCommand(Guid UserId, Guid PlaylistId, Guid SongId) : ICommand<Result>;