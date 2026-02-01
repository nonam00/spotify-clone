using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Moderators.Queries.LoginByCredentials;

public record LoginByCredentialsQuery(string Email, string Password) : IQuery<Result<string>>;