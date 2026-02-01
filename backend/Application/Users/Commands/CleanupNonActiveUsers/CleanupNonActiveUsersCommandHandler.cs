using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Interfaces;
using Domain.Common;

namespace Application.Users.Commands.CleanupNonActiveUsers;

public class CleanupNonActiveUsersCommandHandler : ICommandHandler<CleanupNonActiveUsersCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CleanupNonActiveUsersCommandHandler(IUsersRepository usersRepository, IUnitOfWork unitOfWork)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CleanupNonActiveUsersCommand command, CancellationToken cancellationToken)
    {
        var nonActiveUsers = await _usersRepository.GetNonActiveList(cancellationToken);
        
        if (nonActiveUsers.Count != 0)
        {
            _usersRepository.DeleteRange(nonActiveUsers);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        return Result.Success();
    }
}