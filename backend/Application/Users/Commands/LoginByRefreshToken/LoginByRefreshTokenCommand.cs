using Domain.Common;
using Application.Shared.Messaging;
using Application.Shared.Models;

namespace Application.Users.Commands.LoginByRefreshToken;

public record LoginByRefreshTokenCommand(string RefreshToken) : IQuery<Result<TokenPair>>;