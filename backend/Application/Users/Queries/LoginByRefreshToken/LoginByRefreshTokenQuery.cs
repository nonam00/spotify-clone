using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Models;

namespace Application.Users.Queries.LoginByRefreshToken;

public record LoginByRefreshTokenQuery(string RefreshToken) : IQuery<Result<TokenPair>>;