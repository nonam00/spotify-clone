using Microsoft.Extensions.Logging;

using Domain.Models;
using Domain.ValueObjects;
using Application.Shared.Data;
using Application.Shared.Exceptions;
using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    
    public CreateUserCommandHandler(
        IUsersRepository usersRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork, ILogger<CreateUserCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var checkUser = await _usersRepository.GetByEmail(request.Email, cancellationToken);
        
        if (checkUser != null)
        {
            if (!checkUser.IsActive)
            {
                _logger.LogInformation(               
                    "Tried to create user with email {email} which is already exists but not active.",
                    request.Email);
                throw new LoginException("Activate your account!");
            }
            _logger.LogInformation(
                "Tried to create user with email {email} which is already active.",
                request.Email);
            throw new LoginException("User with this email already exits");
        }

        var hashedPassword = _passwordHasher.Generate(request.Password);

        var email = new Email(request.Email);
        var passwordHash = new PasswordHash(hashedPassword);
        
        var user = User.Create(email, passwordHash);
        
        await _usersRepository.Add(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}