using Application.Users.Interfaces;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, string?>
{
    private readonly IUsersRepository _usersRepository;

    public UpdateUserCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<string?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetById(request.UserId, cancellationToken);
        
        user.FullName = request.FullName ?? user.FullName;
        
        string? oldAvatarPath = null;
        if (request.AvatarPath is not null)
        {
            oldAvatarPath = user.AvatarPath;
            user.AvatarPath = request.AvatarPath;
        }

        await _usersRepository.Update(user, cancellationToken);

        return oldAvatarPath;
    }
}