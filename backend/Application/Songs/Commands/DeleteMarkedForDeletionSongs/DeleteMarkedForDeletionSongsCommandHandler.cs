using Domain.Common;
using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;

namespace Application.Songs.Commands.DeleteMarkedForDeletionSongs;

public class DeleteMarkedForDeletionSongsCommandHandler : ICommandHandler<DeleteMarkedForDeletionSongsCommand, Result>
{
    private readonly ISongsRepository _songsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMarkedForDeletionSongsCommandHandler(ISongsRepository songsRepository, IUnitOfWork unitOfWork)
    {
        _songsRepository = songsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteMarkedForDeletionSongsCommand songsCommand, CancellationToken cancellationToken)
    {
        var markedForDeletion = await _songsRepository
            .GetMarkedForDeletion(cancellationToken)
            .ConfigureAwait(false);
        
        if (markedForDeletion.Count != 0)
        {
            _songsRepository.DeleteRange(markedForDeletion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}