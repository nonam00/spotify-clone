using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Shared.Models;

namespace Application.Users.Commands.LoginByCredentials;

public record LoginByCredentialsCommand(string Email, string Password) : IQuery<Result<TokenPair>>;