using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Users.Queries.CheckLike;

public record CheckLikeQuery(Guid UserId, Guid SongId) : IQuery<Result<bool>>;