using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Models;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Errors;
using Application.Songs.Interfaces;
using Application.Moderators.Interfaces;
using Application.Moderators.Errors;

namespace Application.Moderators.Commands.DeleteSong;

public class DeleteSongCommandHandler : ICommandHandler<DeleteSongCommand, Result>
{
    private readonly IModeratorsRepository _moderatorsRepository;
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSongCommandHandler> _logger;

    public DeleteSongCommandHandler(
        IModeratorsRepository moderatorsRepository,
        ISongsRepository songsRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteSongCommandHandler> logger)
    {
        _moderatorsRepository = moderatorsRepository;
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteSongCommand command, CancellationToken cancellationToken)
    {
        var moderator = await _moderatorsRepository.GetById(command.ModeratorId, cancellationToken);

        if (moderator is null)
        {
            _logger.LogError(
                "Tried to delete song {SongId} but moderator {ModeratorId} doesnt exist",
                command.SongId, command.ModeratorId);
            return Result.Failure(ModeratorErrors.NotFound);
        }

        if (!moderator.IsActive)
        {
            _logger.LogError(
                "Tried to delete song {SongId} but moderator {ModeratorId} is not active",
                command.SongId, command.ModeratorId);
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!moderator.Permissions.CanManageContent)
        {
            _logger.LogWarning(
                "Moderator {ModeratorId} tried to delete song {SongId} but doesnt have permission to manage content",
                command.ModeratorId, command.SongId);
            return Result.Failure(ModeratorDomainErrors.CannotManageContent);
        }
        
        var song = await _songsRepository.GetById(command.SongId, cancellationToken);

        if (song is null)
        {
            _logger.LogError(
                "Moderator {ModeratorId} tried to delete song {SongId} but it doesnt exist.",
                command.ModeratorId, command.SongId);
            return Result.Failure(SongErrors.NotFound);
        }
        
        var deletionResult = moderator.DeleteSong(song);
        if (deletionResult.IsFailure)
        {  
            _logger.LogError(
                "Moderator {ModeratorId} tried to delete song {SongId}" +
                " but domain error occurred: {DomainErrorDescription}.",
                command.ModeratorId, command.SongId, deletionResult.Error.Description);
            return deletionResult;
        }
        
        _songsRepository.Update(song);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "Song {SongId} was successfully marked for deletion by moderator {ModeratorId}.",
            command.SongId, command.ModeratorId);
        
        return Result.Success();
    }
}