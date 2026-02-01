using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Commands.LikeSong;

public class LikeSongCommandHandler : ICommandHandler<LikeSongCommand, Result>
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

    public async Task<Result> Handle(LikeSongCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithLikedSongs(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogError(
                "Tried to like song {songId} but user {userId} doesn't exist",
                request.SongId, request.UserId);
            return Result.Failure(UserErrors.NotFound);
        }
        
        if (!user.LikeSong(request.SongId))
        {
            _logger.LogError(
                "User with id {userId} tried to like song {songId} but he already liked this song",
                request.UserId, request.SongId);
            
            return Result.Failure(UserLikeErrors.AlreadyLiked);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}