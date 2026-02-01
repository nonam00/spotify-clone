using Microsoft.Extensions.Logging;

using Domain.ValueObjects;
using Application.Shared.Data;
using Application.Users.Interfaces;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Domain.Common;

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

        if (user == null)
        {
            _logger.LogError("Tried to update profile but user {userId} does not exist", request.UserId);
            return Result.Failure(UserErrors.NotFound);
        }
        
        var newFullName = request.FullName ?? user.FullName;
        var newAvatarPath = new FilePath(request.AvatarPath ?? user.AvatarPath);

        user.UpdateProfile(newFullName, newAvatarPath);
        
        _usersRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}