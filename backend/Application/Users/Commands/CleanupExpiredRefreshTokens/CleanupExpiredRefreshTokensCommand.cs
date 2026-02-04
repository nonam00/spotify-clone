using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Users.Commands.CleanupExpiredRefreshTokens;

public record CleanupExpiredRefreshTokensCommand : ICommand<Result>;