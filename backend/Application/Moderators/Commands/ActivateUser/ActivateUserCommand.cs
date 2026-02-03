using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.ActivateUser;

public record ActivateUserCommand(Guid ModeratorId, Guid UserId) : ICommand<Result>;