using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Models;
using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Errors;
using Application.Songs.Interfaces;

namespace Application.Moderators.Commands.PublishSongs;

public class PublishSongsCommandHandler :  ICommandHandler<PublishSongsCommand, Result>
{
    private readonly IModeratorsRepository _moderatorsRepository;
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PublishSongsCommandHandler> _logger;

    public PublishSongsCommandHandler(
        IModeratorsRepository moderatorsRepository,
        ISongsRepository songsRepository,
        IUnitOfWork unitOfWork,
        ILogger<PublishSongsCommandHandler> logger)
    {
        _moderatorsRepository = moderatorsRepository;
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(PublishSongsCommand command, CancellationToken cancellationToken)
    {
        var moderator = await _moderatorsRepository.GetById(command.ModeratorId, cancellationToken);

        if (moderator is null)
        {
            _logger.LogError("Tried to publish songs but moderator {ModeratorId} doesnt exist", command.ModeratorId);
            return Result.Failure(ModeratorErrors.NotFound);
        }

        if (!moderator.IsActive)
        {
            _logger.LogError("Tried to publish songs but moderator {ModeratorId} is not active", command.ModeratorId);
            return Result.Failure(ModeratorDomainErrors.NotActive);
        }

        if (!moderator.Permissions.CanManageContent)
        {
            _logger.LogWarning(
                "Moderator {ModeratorId} tried to publish songs but doesnt have permission to manage content",
                command.ModeratorId);
            return Result.Failure(ModeratorDomainErrors.CannotManageContent);
        }

        var songs = await _songsRepository.GetListByIds(command.SongIds, cancellationToken);

        if (!songs.Any())
        {
            return Result.Failure(SongErrors.SongsNotFound);
        }

        if (songs.Count != command.SongIds.Count)
        {
            return Result.Failure(SongErrors.SomeSongsNotFound);
        }

        var publishResult = moderator.PublishSongs(songs);
        if (publishResult.IsFailure)
        {
            _logger.LogError(
                "Moderator {ModeratorId} tried to publish songs but domain error occurred: {DomainErrorDescription}.",
                command.ModeratorId, publishResult.Error.Description);
            return publishResult;
        }

        _songsRepository.UpdateRange(songs);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
                
        _logger.LogInformation(
            "{SongCount} songs were successfully published by moderator {ModeratorId}.",
            command.SongIds.Count, command.ModeratorId);

        return Result.Success();
    }
}