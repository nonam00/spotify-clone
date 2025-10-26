using Application.Users.Interfaces;
using Application.Shared.Messaging;

namespace Application.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler : ICommandHandler<UpdatePasswordCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdatePasswordCommandHandler(IUsersRepository usersRepository, IPasswordHasher passwordHasher)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetById(request.UserId, cancellationToken);

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            throw new Exception("Password doesnt match");
        }

        var newPasswordHash = _passwordHasher.Generate(request.NewPassword);
        user.PasswordHash = newPasswordHash;
        
        await _usersRepository.Update(user, cancellationToken);
    }
}