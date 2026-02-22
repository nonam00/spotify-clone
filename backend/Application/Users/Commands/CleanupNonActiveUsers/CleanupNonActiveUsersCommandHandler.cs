using Domain.Common;
using Application.Shared.Data;
using Application.Shared.Integration;
using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.Users.Commands.CleanupNonActiveUsers;

public class CleanupNonActiveUsersCommandHandler : ICommandHandler<CleanupNonActiveUsersCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IFileServicePublisher _fileServicePublisher;
    private readonly IUnitOfWork _unitOfWork;

    public CleanupNonActiveUsersCommandHandler(
        IUsersRepository usersRepository,
        IFileServicePublisher fileServicePublisher,
        IUnitOfWork unitOfWork)
    {
        _usersRepository = usersRepository;
        _fileServicePublisher = fileServicePublisher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CleanupNonActiveUsersCommand command, CancellationToken cancellationToken)
    {
        var nonActiveUsers = await _usersRepository.GetNonActiveList(cancellationToken);
        
        if (nonActiveUsers.Count != 0)
        {
            _usersRepository.DeleteRange(nonActiveUsers);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            var tasks = nonActiveUsers
                .SelectMany(u => u.Playlists)
                .Select(p => _fileServicePublisher.PublishDeleteFileAsync(
                    p.ImagePath, "image", cancellationToken));
        
            await Task.WhenAll(tasks);
        }
        
        return Result.Success();
    }
}