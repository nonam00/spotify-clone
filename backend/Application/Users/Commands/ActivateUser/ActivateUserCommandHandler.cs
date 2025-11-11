using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Commands.ActivateUser;

public class ActivateUserCommandHandler : ICommandHandler<ActivateUserCommand, Result>
{
    private readonly IEmailVerificator _emailVerificator;
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateUserCommandHandler> _logger;
    
    public ActivateUserCommandHandler(
        IEmailVerificator emailVerificator,
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateUserCommandHandler> logger)
    {
        _emailVerificator = emailVerificator;
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var codeVerificationStatus = await _emailVerificator.VerifyCodeAsync(
            request.Email, request.ConfirmationCode);
        
        if (!codeVerificationStatus)
        {
            _logger.LogInformation(
                "Someone tried to activate user account with email {email} using invalid code {confirmationCode}",
                request.Email, request.ConfirmationCode);
            return Result.Failure(UserErrors.InvalidVerificationCode);
        }

        var user = await _usersRepository.GetByEmail(request.Email, cancellationToken);

        if (user == null)
        {
            _logger.LogError(
                "Someone tried to activate non-existing user account with email {email} and code {confirmationCode}", 
                request.Email, request.ConfirmationCode);
            return Result.Failure(UserErrors.NotFoundWithEmail);
        }
        
        user.Activate();
        _usersRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}