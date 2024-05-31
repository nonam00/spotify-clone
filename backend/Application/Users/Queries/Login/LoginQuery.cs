using MediatR;

namespace Application.Users.Queries.Login
{
    public class LoginQuery : IRequest<UserVm>
    {
        public string Email { get; set; }
        public string Password { get; set; }    
    }
}
