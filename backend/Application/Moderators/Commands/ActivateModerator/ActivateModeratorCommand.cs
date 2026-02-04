using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.ActivateModerator;

public record ActivateModeratorCommand(Guid ManagingModeratorId, Guid ModeratorToActivateId) : ICommand<Result>;