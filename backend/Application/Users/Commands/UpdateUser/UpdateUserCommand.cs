using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid UserId, string? FullName, string? AvatarPath) : ICommand<Result>;