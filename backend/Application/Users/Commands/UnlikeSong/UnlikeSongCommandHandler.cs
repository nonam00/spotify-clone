using Microsoft.Extensions.Logging;

using Domain.Common;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Users.Errors;
using Application.Users.Interfaces;
using Domain.Models;

namespace Application.Users.Commands.UnlikeSong;

public class UnlikeSongCommandHandler : ICommandHandler<UnlikeSongCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnlikeSongCommandHandler> _logger;
    
    public UnlikeSongCommandHandler(
        IUsersRepository usersRepository,
        ISongsRepository songsRepository,
        IUnitOfWork unitOfWork,
        ILogger<UnlikeSongCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UnlikeSongCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithLikedSongs(request.UserId, cancellationToken);
        
        if (user is null)
        {
            _logger.LogError(
                "Tried to unlike song {SongId} but user {UserId} doesn't exist", 
                request.SongId, request.UserId);
            return Result.Failure(UserErrors.NotFound);
        }

        if (!user.IsActive)
        {
            _logger.LogError(
                "User {UserId} tried to unlike song {SongId} but user is not active",
                request.UserId, request.SongId);
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        var unlikeSongResult = user.UnlikeSong(request.SongId);
        
        if (unlikeSongResult.IsFailure)
        {
            if (unlikeSongResult.Error.Code == nameof(UserDomainErrors.SongNotLiked))
            {
                _logger.LogError(
                    "User {UserId} tried to unlike song {SongId} but user has not liked this song",
                    request.UserId, request.SongId); 
            }
            else
            {
                _logger.LogError(
                    "User {UserId} tried to unlike song {SongId} but domain error occurred: {DomainErrorDescription}",
                    request.UserId, request.SongId, unlikeSongResult.Error.Description);
            }

            return unlikeSongResult;
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("User {UserId} successfully unliked song {SongId}", request.UserId, request.SongId);
        
        return Result.Success();
    }
}