using MediatR;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<string>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}