using Application.LikedSongs.Interfaces;
using Application.Shared.Messaging;

namespace Application.LikedSongs.Commands.DeleteLikedSong;

public class DeleteLikedSongCommandHandler : ICommandHandler<DeleteLikedSongCommand>
{
    private readonly ILikedSongsRepository _likedSongsRepository;

    public DeleteLikedSongCommandHandler(ILikedSongsRepository likedSongsRepository)
    {
        _likedSongsRepository = likedSongsRepository;
    }

    public async Task Handle(DeleteLikedSongCommand request, CancellationToken cancellationToken)
    {
        await _likedSongsRepository.Delete(request.UserId, request.SongId, cancellationToken);
    }
}