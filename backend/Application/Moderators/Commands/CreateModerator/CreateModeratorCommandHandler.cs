using Microsoft.Extensions.Logging;

using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Domain.Common;
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
        var managingModerator = await _moderatorsRepository.GetById(command.ManagingModeratorId, cancellationToken);

        if (managingModerator == null)
        {
            _logger.LogInformation("Tried to create moderator but managing moderator does not exist.");
            return Result.Failure(ModeratorErrors.NotFound);
        }

        if (!managingModerator.IsActive)
        {
            _logger.LogInformation(
                "Tried to create moderator but managing moderator {ManagingModeratorId} does not exist.",
                command.ManagingModeratorId);
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }
        
        if (!managingModerator.Permissions.CanManageModerators)
        {
            _logger.LogWarning(
                "Tried to create moderator but managing moderator {ManagingModeratorId} cannot manage moderators",
                command.ManagingModeratorId);
            return Result.Failure(ModeratorDomainErrors.CannotManageModerators);
        }
        
        var checkModerator = await _moderatorsRepository.GetByEmail(command.Email, cancellationToken);
        
        if (checkModerator != null)
        {
            if (!checkModerator.IsActive)
            {
                _logger.LogInformation(               
                    "Tried to create moderator with email {email} which is already exists but not active.",
                    command.Email);
                return Result.Failure(ModeratorErrors.AlreadyExistButNotActive);
            }
            
            _logger.LogInformation(
                "Tried to create user with email {email} which is already active.",
                command.Email);
            
            return Result.Failure(ModeratorErrors.AlreadyExist);
        }

        var email = new Email(command.Email);
        var passwordHash = new PasswordHash(_passwordHasher.Generate(command.Password));
        
        var createModeratorResult = managingModerator.CreateModerator(
            email, passwordHash, command.FullName, command.IsSuper);

        if (createModeratorResult.IsFailure)
        {
            return createModeratorResult;
        }
        
        await _moderatorsRepository.Add(createModeratorResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}