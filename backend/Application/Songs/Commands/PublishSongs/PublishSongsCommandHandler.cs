using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;

namespace Application.Songs.Commands.PublishSongs;

public class PublishSongsCommandHandler :  ICommandHandler<PublishSongsCommand, Result>
{
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishSongsCommandHandler(ISongsRepository songsRepository, IUnitOfWork unitOfWork)
    {
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(PublishSongsCommand command, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.GetListByIds(command.SongIds, cancellationToken);

        foreach (var song in songs)
        {
            song.Publish();
        }
        
        if (songs.Count != 0)
        {
            _songsRepository.UpdateRange(songs);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        return Result.Success();
    }
}