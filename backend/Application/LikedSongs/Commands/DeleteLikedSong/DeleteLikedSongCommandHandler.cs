using MediatR;

using Application.LikedSongs.Interfaces;

namespace Application.LikedSongs.Commands.DeleteLikedSong;

public class DeleteLikedSongCommandHandler : IRequestHandler<DeleteLikedSongCommand>
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