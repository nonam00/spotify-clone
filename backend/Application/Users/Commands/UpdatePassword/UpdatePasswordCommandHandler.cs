using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.ValueObjects;
using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Users.Interfaces;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Domain.Models;

namespace Application.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler : ICommandHandler<UpdatePasswordCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePasswordCommandHandler> _logger;

    public UpdatePasswordCommandHandler(
        IUsersRepository usersRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePasswordCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetById(request.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Tried to change password but user with id {UserId} doesn't exist.", request.UserId);
            return Result.Failure(UserErrors.NotFound);
        }

        if (!user.IsActive)
        {
            _logger.LogError("User {UserId} tried to changed password but user is not active.", request.UserId);
            return Result.Failure(UserDomainErrors.NotActive);
        }

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            _logger.LogInformation(
                "User {UserId} tried to change password" +
                " but provided password doesn't match with the actual password.",
                request.UserId);
            return Result.Failure(UserErrors.PasswordsMissMatch);
        }

        var newPasswordHash = _passwordHasher.Generate(request.NewPassword);
        var passwordHash = new PasswordHash(newPasswordHash);
        
        var changePasswordResult = user.ChangePassword(passwordHash);
        if (changePasswordResult.IsFailure)
        {
            _logger.LogError(
                "User {UserId} tried to change password but domain error occurred: {DomainErrorDescription}",
                request.UserId, changePasswordResult.Error.Description);
            return changePasswordResult;
        }
        
        _usersRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("User {UserId} successfully changed password", request.UserId);
        
        return Result.Success();
    }
}