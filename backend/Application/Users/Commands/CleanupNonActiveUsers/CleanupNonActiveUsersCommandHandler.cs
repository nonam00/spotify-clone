using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.Users.Commands.CleanupNonActiveUsers;

public class CleanupNonActiveUsersCommandHandler : ICommandHandler<CleanupNonActiveUsersCommand>
{
    private readonly IUsersRepository _usersRepository;

    public CleanupNonActiveUsersCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task Handle(CleanupNonActiveUsersCommand command, CancellationToken cancellationToken)
    {
        await _usersRepository.DeleteNonActive(cancellationToken);
    }
}