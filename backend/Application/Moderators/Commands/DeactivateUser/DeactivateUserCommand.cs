using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.DeactivateUser;

public sealed record DeactivateUserCommand(Guid ModeratorId, Guid UserId) : ICommand<Result>;