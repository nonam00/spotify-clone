using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Moderators.Queries.LoginByCredentials;

public record LoginByCredentialsQuery(string Email, string Password) : IQuery<Result<string>>;