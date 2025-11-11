using Application.Shared.Messaging;
using Application.Playlists.Models;
using Application.Shared.Data;

namespace Application.Playlists.Queries.GetPlaylistById;

public record GetPlaylistByIdQuery(Guid UserId, Guid PlaylistId) : IQuery<Result<PlaylistVm>>;