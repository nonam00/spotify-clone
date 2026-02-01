using Application.Playlists.Models;
using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Playlists.Queries.GetFullPlaylistList;

public record GetFullPlaylistListQuery(Guid UserId) : IQuery<Result<PlaylistListVm>>;