using MediatR;

using Application.Users.Models;

namespace Application.Users.Queries.LoginByRefreshToken;

public class LoginByRefreshTokenQuery : IRequest<TokenPair>
{
    public string RefreshToken { get; init; } = null!;
}