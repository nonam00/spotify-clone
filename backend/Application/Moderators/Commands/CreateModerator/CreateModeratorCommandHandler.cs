using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Models;
using Domain.ValueObjects;
using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;

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

        if (managingModerator is null)
        {
            _logger.LogError(
                "Tried to create moderator but managing moderator {ManagingModeratorId} does not exist.",
                command.ManagingModeratorId);
            return Result.Failure(ModeratorErrors.NotFound);
        }

        if (!managingModerator.IsActive)
        {
            _logger.LogError(
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
        
        if (checkModerator is not null)
        {
            if (!checkModerator.IsActive)
            {
                _logger.LogInformation(               
                    "Managing moderator {ManagingModeratorId} tried to create moderator with email {Email}" +
                    " but moderator {ExistingModeratorWithEmailId} with this email already exists but not active.",
                    command.ManagingModeratorId, command.Email, checkModerator.Id);
                return Result.Failure(ModeratorErrors.AlreadyExistButNotActive);
            }
            
            _logger.LogInformation(
                "Managing moderator {ManagingModeratorId} tried to create moderator with email {Email}" +
                " but moderator {ExistingModeratorWithEmailId} with this email already exists.",
                command.ManagingModeratorId, command.Email, checkModerator.Id);
            
            return Result.Failure(ModeratorErrors.AlreadyExist);
        }

        var email = new Email(command.Email);
        var passwordHash = new PasswordHash(_passwordHasher.Generate(command.Password));
        
        var createModeratorResult = managingModerator.CreateModerator(
            email, passwordHash, command.FullName, command.IsSuper);

        if (createModeratorResult.IsFailure)
        {
            _logger.LogError(
                "Managing moderator {ManagingModeratorId} tried to create moderator" +
                " but domain error occurred: {DomainErrorDescription}.",
                command.ManagingModeratorId, createModeratorResult.Error.Description);
            return createModeratorResult;
        }
        
        await _moderatorsRepository.Add(createModeratorResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}