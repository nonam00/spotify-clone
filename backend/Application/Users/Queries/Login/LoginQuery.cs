using MediatR;

namespace Application.Users.Queries.Login
{
    public class LoginQuery : IRequest<string>
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
