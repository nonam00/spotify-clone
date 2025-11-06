using Application.Shared.Messaging;

namespace Application.Users.Queries.CheckLike;

public record CheckLikeQuery(Guid UserId, Guid SongId) : IQuery<bool>;