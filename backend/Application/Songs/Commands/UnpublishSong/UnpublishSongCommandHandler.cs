using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Errors;
using Application.Songs.Interfaces;
using Domain.Common;

namespace Application.Songs.Commands.UnpublishSong;

public class UnpublishSongCommandHandler :  ICommandHandler<UnpublishSongCommand, Result>
{
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnpublishSongCommandHandler(ISongsRepository songsRepository, IUnitOfWork unitOfWork)
    {
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UnpublishSongCommand command, CancellationToken cancellationToken)
    {
        var song = await _songsRepository.GetById(command.Id, cancellationToken);

        if (song == null)
        {
            return Result.Failure(SongErrors.NotFound);
        }
        
        song.Unpublish();
        
        _songsRepository.Update(song);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}