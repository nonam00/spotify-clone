using MediatR;

using Application.Common.Exceptions;
using Application.Users.Interfaces;

namespace Application.Users.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, string>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public LoginQueryHandler( IUsersRepository usersRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<string> Handle(LoginQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByEmail(request.Email, cancellationToken)
                   ?? throw new LoginException("Invalid email or password. Please try again.");
            
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new LoginException("Invalid email or password. Please try again.");
        }

        var token = _jwtProvider.GenerateToken(user);

        return token;
    }
}