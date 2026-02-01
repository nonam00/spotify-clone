using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.UpdateUserStatus;

public record UpdateUserStatusCommand(Guid UserId, bool IsActive) : ICommand<Result>;