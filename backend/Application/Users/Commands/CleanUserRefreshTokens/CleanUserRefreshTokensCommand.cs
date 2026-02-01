using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.CleanUserRefreshTokens;

public record CleanUserRefreshTokensCommand(Guid UserId) : ICommand<Result>;