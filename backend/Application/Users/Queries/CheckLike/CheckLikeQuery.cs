using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Queries.CheckLike;

public record CheckLikeQuery(Guid UserId, Guid SongId) : IQuery<Result<bool>>;