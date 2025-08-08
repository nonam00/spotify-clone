using Domain;
using Application.Playlists.Models;

namespace Application.Playlists.Interfaces;

public interface IPlaylistsRepository
{
    Task<PlaylistVm> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<List<PlaylistVm>> GetList(Guid userId, CancellationToken cancellationToken = default);
    Task<List<PlaylistVm>> TakeList(Guid userId, int count, CancellationToken cancellationToken = default);
    Task<int> GetCount(Guid userId, CancellationToken cancellationToken = default);
    Task Add(Playlist playlist, CancellationToken cancellationToken = default);
    Task Delete(Guid playlistId, Guid userId, CancellationToken cancellationToken = default);
    Task Update(Guid playlistId, Guid userId, string title, string? description, string? imagePath,
        CancellationToken cancellationToken = default);
}