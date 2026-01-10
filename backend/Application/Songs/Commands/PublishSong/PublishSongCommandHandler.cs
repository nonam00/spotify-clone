using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Errors;
using Application.Songs.Interfaces;

namespace Application.Songs.Commands.PublishSong;

public class PublishSongCommandHandler :  ICommandHandler<PublishSongCommand, Result>
{
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishSongCommandHandler(ISongsRepository songsRepository, IUnitOfWork unitOfWork)
    {
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(PublishSongCommand command, CancellationToken cancellationToken)
    {
        var song = await _songsRepository.GetById(command.Id, cancellationToken);

        if (song == null)
        {
            return Result.Failure(SongErrors.NotFound);
        }
        
        song.Publish();
        
        _songsRepository.Update(song);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}