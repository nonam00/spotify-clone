using Application.Shared.Messaging;
using Application.Shared.Models;
using Domain.Common;

namespace Application.Users.Commands.LoginByRefreshToken;

public record LoginByRefreshTokenCommand(string RefreshToken) : IQuery<Result<TokenPair>>;