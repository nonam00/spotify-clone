using Domain.Models;
using Application.Playlists.Models;

namespace Application.Playlists.Interfaces;

public interface IPlaylistsRepository
{
    Task<Playlist?> GetById(Guid playlistId, CancellationToken cancellationToken = default);
    Task<Playlist?> GetByIdWithSongs(Guid playlistId, CancellationToken cancellationToken = default);
    Task<List<PlaylistVm>> GetList(Guid userId, CancellationToken cancellationToken = default);
    Task<List<PlaylistVm>> TakeList(Guid userId, int count, CancellationToken cancellationToken = default);
    Task<List<PlaylistVm>> GetListWithoutSong(Guid userId, Guid songId, CancellationToken cancellationToken = default);
    void Update(Playlist playlist);
}