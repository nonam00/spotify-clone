using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Users.Commands.UpdatePassword;

public record UpdatePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : ICommand<Result>;