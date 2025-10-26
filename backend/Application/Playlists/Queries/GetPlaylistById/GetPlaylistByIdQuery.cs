using Application.Shared.Messaging;

using Application.Playlists.Models;

namespace Application.Playlists.Queries.GetPlaylistById;

public record GetPlaylistByIdQuery(Guid UserId, Guid PlaylistId) : IQuery<PlaylistVm>;