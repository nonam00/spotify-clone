using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Shared.Models;

namespace Application.Users.Commands.LoginByRefreshToken;

public record LoginByRefreshTokenCommand(string RefreshToken) : IQuery<Result<TokenPair>>;