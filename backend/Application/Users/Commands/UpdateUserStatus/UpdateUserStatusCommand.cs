using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Users.Commands.UpdateUserStatus;

public record UpdateUserStatusCommand(Guid UserId, bool IsActive) : ICommand<Result>;