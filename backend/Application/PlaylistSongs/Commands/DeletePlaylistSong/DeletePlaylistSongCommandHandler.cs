using Application.Shared.Messaging;

using Application.PlaylistSongs.Interfaces;

namespace Application.PlaylistSongs.Commands.DeletePlaylistSong;

public class DeletePlaylistSongCommandHandler: ICommandHandler<DeletePlaylistSongCommand>
{
    private readonly IPlaylistsSongsRepository _playlistsSongsRepository;

    public DeletePlaylistSongCommandHandler(IPlaylistsSongsRepository playlistsSongsRepository)
    {
        _playlistsSongsRepository = playlistsSongsRepository;
    }

    public async Task Handle(DeletePlaylistSongCommand request, CancellationToken cancellationToken)
    {
        await _playlistsSongsRepository.Delete(
            request.UserId, request.PlaylistId, request.SongId, cancellationToken);
    }
}