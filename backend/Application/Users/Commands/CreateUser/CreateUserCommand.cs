using Application.Shared.Messaging;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommand : ICommand
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}