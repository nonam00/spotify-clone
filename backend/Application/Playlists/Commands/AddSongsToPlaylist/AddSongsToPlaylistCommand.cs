using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Playlists.Commands.AddSongsToPlaylist;

public record AddSongsToPlaylistCommand(Guid UserId, Guid PlaylistId, List<Guid> SongIds) : ICommand<Result>;