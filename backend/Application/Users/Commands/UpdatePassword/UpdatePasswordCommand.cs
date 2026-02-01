using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.UpdatePassword;

public record UpdatePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : ICommand<Result>;