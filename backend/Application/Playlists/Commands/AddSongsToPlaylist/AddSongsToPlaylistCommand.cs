using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Playlists.Commands.AddSongsToPlaylist;

public record AddSongsToPlaylistCommand(Guid UserId, Guid PlaylistId, List<Guid> SongIds) : ICommand<Result>;