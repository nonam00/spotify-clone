using Application.Playlists.Errors;
using Application.Playlists.Interfaces;
using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Playlists.Commands.ReorderSongsInPlaylist;

public class ReorderSongsInPlaylistCommandHandler : ICommandHandler<ReorderSongsInPlaylistCommand, Result>
{
    private readonly IPlaylistsRepository  _playlistsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReorderSongsInPlaylistCommandHandler(IPlaylistsRepository playlistsRepository, IUnitOfWork unitOfWork)
    {
        _playlistsRepository = playlistsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReorderSongsInPlaylistCommand command, CancellationToken cancellationToken)
    {
        var playlist = await _playlistsRepository.GetByIdWithSongs(command.PlaylistId, cancellationToken);

        if (playlist == null)
        {
            return Result.Failure(PlaylistErrors.NotFound);
        }

        playlist.ReorderSongs(command.SongIds);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}