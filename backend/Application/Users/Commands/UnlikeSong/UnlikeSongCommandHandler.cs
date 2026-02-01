using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Users.Errors;
using Application.Users.Interfaces;
using Domain.Common;

namespace Application.Users.Commands.UnlikeSong;

public class UnlikeSongCommandHandler : ICommandHandler<UnlikeSongCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnlikeSongCommandHandler> _logger;
    
    public UnlikeSongCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        ILogger<UnlikeSongCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UnlikeSongCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithLikedSongs(request.UserId, cancellationToken);
        
        if (user == null)
        {
            _logger.LogError(
                "Tried to unlike song {songId} but user {userId} doesn't exist", 
                request.SongId, request.UserId);
            return Result.Failure(UserErrors.NotFound);
        }
        
        if (!user.UnlikeSong(request.SongId))
        {
            _logger.LogError(
                "User with id {userId} tried to like song {songId} but he has not liked this song",
                request.UserId, request.SongId); 
            return Result.Failure(UserLikeErrors.NotLiked);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}