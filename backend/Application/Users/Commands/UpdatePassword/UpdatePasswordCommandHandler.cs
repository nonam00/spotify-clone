using Microsoft.Extensions.Logging;

using Domain.ValueObjects;
using Application.Shared.Data;
using Application.Users.Interfaces;
using Application.Shared.Messaging;

namespace Application.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler : ICommandHandler<UpdatePasswordCommand>
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

    public async Task Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetById(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogError("Tried to change password but user with id {userId} doesn't exist", request.UserId);
            throw new ArgumentException("User doesn't exist");
        }

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            _logger.LogInformation(
                "User {userId} tried to change password but password {requestCurrentPassword} doesn't match with the actual password",
                request.UserId, request.CurrentPassword);
            throw new ArgumentException("Password doesn't match");
        }

        var newPasswordHash = _passwordHasher.Generate(request.NewPassword);
        var passwordHash = new PasswordHash(newPasswordHash);
        user.ChangePassword(passwordHash);
        
        _usersRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}