using Application.Shared.Messaging;

using Application.PlaylistSongs.Interfaces;

namespace Application.PlaylistSongs.Commands.CreatePlaylistSong;

public class CreatePlaylistSongCommandHandler : ICommandHandler<CreatePlaylistSongCommand, string>
{
    private readonly IPlaylistsSongsRepository _playlistsSongsRepository;

    public CreatePlaylistSongCommandHandler(IPlaylistsSongsRepository playlistsSongsRepository)
    {
        _playlistsSongsRepository = playlistsSongsRepository;
    }

    public async Task<string> Handle(CreatePlaylistSongCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _playlistsSongsRepository.Create(
            request.UserId, request.PlaylistId, request.SongId, cancellationToken);
            
        return result;
    }
}