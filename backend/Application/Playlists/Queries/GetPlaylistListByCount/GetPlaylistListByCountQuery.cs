using Application.Playlists.Models;
using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Playlists.Queries.GetPlaylistListByCount;

public record GetPlaylistListByCountQuery(Guid UserId, int Count) : IQuery<Result<PlaylistListVm>>;