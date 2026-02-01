using Application.Shared.Messaging;
using Application.Shared.Models;
using Domain.Common;

namespace Application.Users.Commands.LoginByCredentials;

public record LoginByCredentialsCommand(string Email, string Password) : IQuery<Result<TokenPair>>;