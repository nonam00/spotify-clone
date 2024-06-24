using MediatR;

namespace Application.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<string>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
