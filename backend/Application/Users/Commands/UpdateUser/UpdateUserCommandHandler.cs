using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Models;
using Domain.ValueObjects;
using Application.Shared.Data;
using Application.Users.Interfaces;
using Application.Shared.Messaging;
using Application.Users.Errors;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetById(request.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Tried to update profile but user {UserId} does not exist.", request.UserId);
            return Result.Failure(UserErrors.NotFound);
        }

        if (!user.IsActive)
        {
            _logger.LogError("User {UserId} tried to update profile but user is not active.", request.UserId);
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        var newFullName = request.FullName ?? user.FullName;
        var newAvatarPath = new FilePath(request.AvatarPath ?? user.AvatarPath);

        var updateProfileResult = user.UpdateProfile(newFullName, newAvatarPath);
        if (updateProfileResult.IsFailure)
        {
            _logger.LogError(
                "User {UserId} tried to update profile" +
                " but domain error occurred: {DomainErrorDescription}.",
                request.UserId, updateProfileResult.Error.Description);
            return updateProfileResult;
        }
        
        _usersRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} successfully updated profile", request.UserId);
        
        return Result.Success();
    }
}