using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Users.Commands.CleanupRefreshTokens;

public record CleanupRefreshTokensCommand : ICommand<Result>;