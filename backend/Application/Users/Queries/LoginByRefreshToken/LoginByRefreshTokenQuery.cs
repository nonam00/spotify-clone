using Application.Shared.Messaging;

using Application.Users.Models;

namespace Application.Users.Queries.LoginByRefreshToken;

public class LoginByRefreshTokenQuery : IQuery<TokenPair>
{
    public string RefreshToken { get; init; } = null!;
}