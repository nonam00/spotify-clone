using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.DeleteRefreshToken;

public record DeleteRefreshTokenCommand(string RefreshToken) : ICommand<Result>;