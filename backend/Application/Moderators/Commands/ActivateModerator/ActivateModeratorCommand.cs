using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Moderators.Commands.ActivateModerator;

public sealed record ActivateModeratorCommand(Guid ManagingModeratorId, Guid ModeratorToActivateId) : ICommand<Result>;