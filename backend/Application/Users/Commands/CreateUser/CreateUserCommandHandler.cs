using MediatR;

using Domain;

using Application.Common.Exceptions;
using Application.Users.Interfaces;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public CreateUserCommandHandler(
        IUsersRepository usersRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<string> Handle(CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        if (await _usersRepository.CheckIfExists(request.Email, cancellationToken))
        {
            throw new LoginException("User with this email already exits");
        }

        var hashedPassword = _passwordHasher.Generate(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = hashedPassword
        };

        await _usersRepository.Add(user, cancellationToken);

        var jwtToken = _jwtProvider.GenerateToken(user);

        return jwtToken;
    }
}