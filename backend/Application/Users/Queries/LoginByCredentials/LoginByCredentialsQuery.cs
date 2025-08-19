using MediatR;

using Application.Users.Models;

namespace Application.Users.Queries.LoginByCredentials;

public class LoginByCredentialsQuery : IRequest<TokenPair>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}