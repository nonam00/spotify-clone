using MediatR;

using Application.Users.Interfaces;

namespace Application.Users.Commands.ActivateUser;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand>
{
    private readonly IEmailVerificator _emailVerificator;
    private readonly IUsersRepository _usersRepository;

    public ActivateUserCommandHandler(IEmailVerificator emailVerificator, IUsersRepository usersRepository)
    {
        _emailVerificator = emailVerificator;
        _usersRepository = usersRepository;
    }

    public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var codeVerificationStatus = await _emailVerificator.VerifyCodeAsync(
            request.Email, request.ConfirmationCode);
        
        if (!codeVerificationStatus)
        {
            throw new Exception("Invalid code");
        }

        var user = await _usersRepository.GetByEmail(request.Email, cancellationToken)
            ?? throw new Exception($"User with email {request.Email} doesn't exist");
        
        user.IsActive = true;

        await _usersRepository.Update(user, cancellationToken);
    }
}