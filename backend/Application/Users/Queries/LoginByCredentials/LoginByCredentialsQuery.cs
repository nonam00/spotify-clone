using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Shared.Models;

namespace Application.Users.Queries.LoginByCredentials;

public record LoginByCredentialsQuery(string Email, string Password) : IQuery<Result<TokenPair>>;