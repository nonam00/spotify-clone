using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.UpdateModeratorStatus;

public record UpdateModeratorStatusCommand(Guid ModeratorId, bool IsActive) : ICommand<Result>;

