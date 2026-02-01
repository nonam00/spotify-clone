using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Errors;
using Application.Songs.Interfaces;
using Domain.Common;

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

        if (songs.Count == 0)
        {
            return Result.Failure(SongErrors.SongsNotFound);
        }
        
        foreach (var song in songs)
        {
            song.Delete();
        }
            
        _songsRepository.DeleteRange(songs);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}