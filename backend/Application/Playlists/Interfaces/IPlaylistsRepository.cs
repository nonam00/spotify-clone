using Domain;
using Application.Playlists.Models;

namespace Application.Playlists.Interfaces;

public interface IPlaylistsRepository
{
    Task<Playlist> GetById(Guid playlistId, Guid userId, CancellationToken cancellationToken = default);
    Task<PlaylistVm> GetVmById(Guid playlistId, Guid userId, CancellationToken cancellationToken = default);
    Task<List<PlaylistVm>> GetList(Guid userId, CancellationToken cancellationToken = default);
    Task<List<PlaylistVm>> TakeList(Guid userId, int count, CancellationToken cancellationToken = default);
    Task<int> GetCount(Guid userId, CancellationToken cancellationToken = default);
    Task Add(Playlist playlist, CancellationToken cancellationToken = default);
    Task Delete(Playlist playlist, CancellationToken cancellationToken = default);
    Task Update(Playlist playlist, CancellationToken cancellationToken = default);
}