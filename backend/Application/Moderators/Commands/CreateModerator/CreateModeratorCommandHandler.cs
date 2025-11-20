using Microsoft.Extensions.Logging;

using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Moderators.Commands.CreateModerator;

public class CreateModeratorCommandHandler : ICommandHandler<CreateModeratorCommand, Result>
{
    private readonly IModeratorsRepository _moderatorsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<CreateModeratorCommandHandler> _logger;

    public CreateModeratorCommandHandler(
        IModeratorsRepository moderatorsRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ILogger<CreateModeratorCommandHandler> logger)
    {
        _moderatorsRepository = moderatorsRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result> Handle(CreateModeratorCommand command, CancellationToken cancellationToken)
    {
        var checkModerator = await _moderatorsRepository.GetByEmail(command.Email, cancellationToken);
        
        if (checkModerator != null)
        {
            if (!checkModerator.IsActive)
            {
                _logger.LogInformation(               
                    "Tried to create user with email {email} which is already exists but not active.",
                    command.Email);
                return Result.Failure(ModeratorErrors.AlreadyExistButNotActive);
            }
            _logger.LogInformation(
                "Tried to create user with email {email} which is already active.",
                command.Email);
            return Result.Failure(ModeratorErrors.AlreadyExist);
        }
        
        var permissions = command.IsSuper
            ? ModeratorPermissions.CreateSuperAdmin()
            : ModeratorPermissions.CreateDefault();

        var email = new Email(command.Email);
        var passwordHash = new PasswordHash(_passwordHasher.Generate(command.Password));
        
        var moderator = Moderator.Create(email, passwordHash, command.FullName, permissions);
        
        await _moderatorsRepository.Add(moderator, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}