using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.ActivateUser;

public sealed record ActivateUserCommand(Guid ModeratorId, Guid UserId) : ICommand<Result>;