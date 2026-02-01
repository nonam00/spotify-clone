using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.UpdateModeratorStatus;

public record UpdateModeratorStatusCommand(Guid ModeratorId, bool IsActive) : ICommand<Result>;