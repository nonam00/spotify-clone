using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.CleanupRefreshTokens;

public record CleanupRefreshTokensCommand : ICommand<Result>;