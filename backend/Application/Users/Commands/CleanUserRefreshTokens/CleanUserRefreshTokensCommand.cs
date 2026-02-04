using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Users.Commands.CleanUserRefreshTokens;

public record CleanUserRefreshTokensCommand(Guid UserId) : ICommand<Result>;