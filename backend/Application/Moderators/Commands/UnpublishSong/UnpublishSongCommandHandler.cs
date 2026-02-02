using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Domain.Common;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Errors;
using Application.Songs.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Moderators.Commands.UnpublishSong;

public class UnpublishSongCommandHandler :  ICommandHandler<UnpublishSongCommand, Result>
{
    private readonly IModeratorsRepository _moderatorsRepository;
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnpublishSongCommandHandler> _logger;

    public UnpublishSongCommandHandler(
        IModeratorsRepository moderatorsRepository,
        ISongsRepository songsRepository,
        IUnitOfWork unitOfWork,
        ILogger<UnpublishSongCommandHandler> logger)
    {
        _moderatorsRepository = moderatorsRepository;
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UnpublishSongCommand command, CancellationToken cancellationToken)
    {
        var moderator = await _moderatorsRepository.GetById(command.ModeratorId, cancellationToken);

        if (moderator is null)
        {
            _logger.LogError(
                "Tried to unpublish song {SongId} but moderator {ModeratorId} doesnt exist",
                command.SongId, command.ModeratorId);
            return Result.Failure(ModeratorErrors.NotFound);
        }

        if (!moderator.IsActive)
        {
            _logger.LogError(
                "Tried to unpublish song {SongId} but moderator {ModeratorId} is not active",
                command.SongId, command.ModeratorId);
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!moderator.Permissions.CanManageContent)
        {
            _logger.LogWarning(
                "Moderator {ModeratorId} tried to unpublish song {SongId} but doesnt have permission to manage content",
                command.ModeratorId, command.SongId);
            return Result.Failure(ModeratorDomainErrors.CannotManageContent);
        }
        
        var song = await _songsRepository.GetById(command.SongId, cancellationToken);
        
        if (song is null)
        {
            _logger.LogError(
                "Moderator {ModeratorId} tried to unpublish song {SongId} but it doesnt exist.",
                command.ModeratorId, command.SongId);
            return Result.Failure(SongErrors.NotFound);
        }
        
        var publishSongResult = moderator.UnpublishSong(song);
        if (publishSongResult.IsFailure)
        {  
            _logger.LogError(
                "Moderator {ModeratorId} tried to unpublish song {SongId}" +
                " but domain error occurred: {DomainErrorDescription}.",
                command.ModeratorId, command.SongId, publishSongResult.Error.Description);
            return publishSongResult;
        }
        
        _songsRepository.Update(song);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "Song {SongId} was successfully unpublished by moderator {ModeratorId}.",
            command.SongId, command.ModeratorId);
        
        return Result.Success();
    }
}