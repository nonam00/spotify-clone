using MediatR;

using Domain;
using Application.Common.Exceptions;
using Application.Users.Interfaces;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailVerificator _emailVerificator;
    public CreateUserCommandHandler(
        IUsersRepository usersRepository,
        IPasswordHasher passwordHasher,
        IEmailVerificator emailVerificator)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _emailVerificator = emailVerificator;
    }

    public async Task Handle(CreateUserCommand request,
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
            PasswordHash = hashedPassword,
            IsActive = false
        };

        await _usersRepository.Add(user, cancellationToken);

        var verificationCode = _emailVerificator.GenerateCode();
        await _emailVerificator.StoreCodeAsync(request.Email, verificationCode);
        await _emailVerificator.SendCodeAsync(request.Email, verificationCode, cancellationToken);
    }
}