using Application.Shared.Messaging;

using Application.Users.Models;

namespace Application.Users.Queries.LoginByCredentials;

public class LoginByCredentialsQuery : IQuery<TokenPair>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}