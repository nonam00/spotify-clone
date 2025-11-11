using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Users.Commands.DeleteRefreshToken;

public record DeleteRefreshTokenCommand(string RefreshToken) : ICommand<Result>;