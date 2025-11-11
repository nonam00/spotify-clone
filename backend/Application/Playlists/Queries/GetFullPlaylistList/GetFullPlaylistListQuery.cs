using Application.Playlists.Models;
using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Playlists.Queries.GetFullPlaylistList;

public record GetFullPlaylistListQuery(Guid UserId) : IQuery<Result<PlaylistListVm>>;