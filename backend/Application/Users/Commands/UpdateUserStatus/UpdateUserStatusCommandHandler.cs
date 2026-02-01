using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;
using Domain.Common;

namespace Application.Users.Commands.UpdateUserStatus;

public class UpdateUserStatusCommandHandler : ICommandHandler<UpdateUserStatusCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserStatusCommandHandler> _logger;

    public UpdateUserStatusCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserStatusCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateUserStatusCommand command, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetById(command.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found while updating status", command.UserId);
            return Result.Failure(UserErrors.NotFound);
        }

        if (command.IsActive)
        {
            user.Activate();
        }
        else
        {
            user.Deactivate();
        }

        _usersRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} status changed to {Status}", command.UserId, command.IsActive);

        return Result.Success();
    }
}