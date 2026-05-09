using Microsoft.Extensions.Logging;

using Domain.Common;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Errors;
using Application.Songs.Interfaces;

namespace Application.Songs.Commands.UpdateTranscribeInformation;

public partial class UpdateTranscribeInformationCommandHandler : ICommandHandler<UpdateTranscribeInformationCommand, Result>
{
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTranscribeInformationCommandHandler> _logger;

    public UpdateTranscribeInformationCommandHandler(
        ISongsRepository songsRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTranscribeInformationCommandHandler> logger)
    {
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateTranscribeInformationCommand command, CancellationToken cancellationToken)
    {
        var song = await _songsRepository.GetByIdWithLyricsSegments(command.SongId, cancellationToken);
        if (song is null)
        {
            _logger.LogError("Tried to get song {songId} but it does not exist", command.SongId); 
            return Result.Failure(SongErrors.SongsNotFound);
        }
        
        song.UpdateTranscribeInformation(command.ContainsExplicitContent, command.LyricsSegments);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        LogSuccessfullyUpdatedTranscribeInformation(_logger, command.SongId, command.ContainsExplicitContent);
            
        return Result.Success();
    }
    
    [LoggerMessage(LogLevel.Debug, "Successfully changed transcribe information for song {SongId}, contains explicit content: {ContainsExplicitContent}.")]
    private static partial void LogSuccessfullyUpdatedTranscribeInformation(ILogger logger, Guid songId, bool containsExplicitContent);
}