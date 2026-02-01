using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.DeactivateModerator;

public record DeactivateModeratorCommand(Guid ManagingModeratorId, Guid ModeratorToDeactivateId) : ICommand<Result>;