using Application.Shared.Messaging;

namespace Application.Users.Commands.CreateUser;

public record CreateUserCommand(string Email, string Password) : ICommand;