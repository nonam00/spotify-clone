using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.SendRestoreToken;

public record SendRestoreTokenCommand(string Email) : ICommand<Result>;