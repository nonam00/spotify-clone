using MediatR;

using Domain;
using Application.LikedSongs.Interfaces;

namespace Application.LikedSongs.Commands.CreateLikedSong;

public class CreateLikedSongCommandHandler : IRequestHandler<CreateLikedSongCommand, string>
{
    private readonly ILikedSongsRepository _likedSongsRepository;

    public CreateLikedSongCommandHandler(ILikedSongsRepository likedSongsRepository)
    {
        _likedSongsRepository = likedSongsRepository;
    }

    public async Task<string> Handle(CreateLikedSongCommand request, CancellationToken cancellationToken)
    {
        var likedSong = new LikedSong
        {
            UserId = request.UserId,
            SongId = request.SongId
        };

        await _likedSongsRepository.Add(likedSong, cancellationToken);

        return $"{likedSong.UserId}:{likedSong.SongId}";
    }
}