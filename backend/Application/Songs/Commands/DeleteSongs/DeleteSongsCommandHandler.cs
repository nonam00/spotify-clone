using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;

namespace Application.Songs.Commands.DeleteSongs;

public class DeleteSongsCommandHandler : ICommandHandler<DeleteSongsCommand, Result>
{
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSongsCommandHandler(ISongsRepository songsRepository, IUnitOfWork unitOfWork)
    {
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteSongsCommand command, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.GetListByIds(command.SongIds, cancellationToken);

        foreach (var song in songs)
        {
            song.Delete();
        }
        
        if (songs.Count != 0)
        {
            _songsRepository.UpdateRange(songs);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _songsRepository.DeleteRange(songs);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        return Result.Success();
    }
}