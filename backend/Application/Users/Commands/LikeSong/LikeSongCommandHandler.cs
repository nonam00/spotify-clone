using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Models;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Errors;
using Application.Songs.Interfaces;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Commands.LikeSong;

public class LikeSongCommandHandler : ICommandHandler<LikeSongCommand, Result>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LikeSongCommandHandler> _logger;
    
    public LikeSongCommandHandler(
        IUsersRepository usersRepository,
        ISongsRepository songsRepository,
        IUnitOfWork unitOfWork,
        ILogger<LikeSongCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(LikeSongCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdWithLikedSongs(request.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError(
                "Tried to like song {SongId} but user {UserId} doesn't exist",
                request.SongId, request.UserId);
            return Result.Failure(UserErrors.NotFound);
        }

        if (!user.IsActive)
        {
            _logger.LogError(
                "User {UserId} tried to like song {SongId} but user is not active.",
                request.UserId, request.SongId);
            return Result.Failure(UserDomainErrors.NotActive);
        }
        
        var song = await _songsRepository.GetById(request.SongId, cancellationToken);

        if (song is null)
        {
            _logger.LogError(
                "User {UserId} tried to like song {SongId} but it doesn't exist",
                request.UserId, request.SongId);
            return Result.Failure(SongErrors.NotFound);
        }
        
        var likeSongResult = user.LikeSong(song);
        if (likeSongResult.IsFailure)
        {
            if (likeSongResult.Error == UserDomainErrors.SongAlreadyLiked)
            {
                _logger.LogError(
                    "User with id {userId} tried to like song {songId} but he already liked this song",
                    request.UserId, request.SongId);
            }
            else
            {
                _logger.LogError(
                    "User with id {userId} tried to like song {songId}" +
                    " but domain error occurred: {DomainErrorDescription}",
                    request.UserId, request.SongId,  likeSongResult.Error.Description);   
            }
            
            return  Result.Failure(likeSongResult.Error);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("User {UserId} successfully liked song {SongId}", request.UserId, request.SongId);
        
        return Result.Success();
    }
}