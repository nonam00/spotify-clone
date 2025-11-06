using Application.Shared.Messaging;

namespace Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid UserId, string? FullName, string? AvatarPath) : ICommand;