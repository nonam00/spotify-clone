using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Models;
using Domain.ValueObjects;
using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    
    public CreateUserCommandHandler(
        IUsersRepository usersRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<CreateUserCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var checkUser = await _usersRepository.GetByEmail(request.Email, cancellationToken);
        
        if (checkUser != null)
        {
            if (!checkUser.IsActive)
            {
                _logger.LogInformation(               
                    "Tried to create user with email {Email} which is already exists but not active.",
                    request.Email);
                return Result.Failure(UserErrors.AlreadyExistButNotActive);
            }
            
            _logger.LogInformation("Tried to create user with email {Email} which is already active.", request.Email);
            return Result.Failure(UserErrors.AlreadyExist);
        }

        var hashedPassword = _passwordHasher.Generate(request.Password);

        var email = new Email(request.Email);
        var passwordHash = new PasswordHash(hashedPassword);
        
        var user = User.Create(email, passwordHash, request.FullName);
        
        await _usersRepository.Add(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Successfully created new user {UserId}.", user.Id);
        
        return Result.Success();
    }
}