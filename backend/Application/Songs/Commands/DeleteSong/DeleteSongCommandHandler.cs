using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Errors;
using Application.Songs.Interfaces;

namespace Application.Songs.Commands.DeleteSong;

public class DeleteSongCommandHandler : ICommandHandler<DeleteSongCommand, Result>
{
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSongCommandHandler(ISongsRepository songsRepository, IUnitOfWork unitOfWork)
    {
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteSongCommand command, CancellationToken cancellationToken)
    {
        var song = await _songsRepository.GetById(command.Id, cancellationToken);

        if (song == null)
        {
            return Result.Failure(SongErrors.NotFound);
        }
        
        song.Delete();
        _songsRepository.Delete(song);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}