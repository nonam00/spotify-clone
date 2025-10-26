using Application.Shared.Messaging;

namespace Application.Users.Commands.ActivateUser;

public record ActivateUserCommand(string Email, string ConfirmationCode) : ICommand;