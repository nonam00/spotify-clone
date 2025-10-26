using Application.Shared.Messaging;

using Application.Users.Models;

namespace Application.Users.Queries.LoginByCredentials;

public record LoginByCredentialsQuery(string Email, string Password) : IQuery<TokenPair>;