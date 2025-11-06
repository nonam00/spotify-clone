using Microsoft.Extensions.Logging;

using Domain.ValueObjects;
using Application.Shared.Data;
using Application.Users.Interfaces;
using Application.Shared.Messaging;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
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

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetById(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogError("Tried to update profile but user {userId} does not exist", request.UserId);
            throw new ArgumentException("User doesn't exist");
        }
        
        var newFullName = request.FullName ?? user.FullName;
        var newAvatarPath = new FilePath(request.AvatarPath ?? user.AvatarPath);

        user.UpdateProfile(newFullName, newAvatarPath);
        
        _usersRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}