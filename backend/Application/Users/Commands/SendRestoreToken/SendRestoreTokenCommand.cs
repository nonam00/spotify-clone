using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Users.Commands.SendRestoreToken;

public record SendRestoreTokenCommand(string Email) : ICommand<Result>;