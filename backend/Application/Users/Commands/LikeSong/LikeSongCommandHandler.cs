using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Interfaces;

namespace Application.Users.Commands.LikeSong;

public class LikeSongCommandHandler : ICommandHandler<LikeSongCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LikeSongCommandHandler> _logger;
    
    public LikeSongCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        ILogger<LikeSongCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(LikeSongCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithSongs(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogError(
                "Tried to like song {songId} but user {userId} doesn't exist",
                request.SongId, request.UserId);
            throw new ArgumentException("User doesn't exist");
        }
        
        if (!user.LikeSong(request.SongId))
        {
            _logger.LogError(
                "User with id {userId} tried to like song {songId} but he already liked this song",
                request.UserId, request.SongId);
            throw new ArgumentException("You already have liked this song");
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}