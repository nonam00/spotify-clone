namespace Application.PlaylistSongs.Interfaces;

public interface IPlaylistsSongsRepository
{
    Task<string> Create(Guid userId, Guid playlistId, Guid songId, CancellationToken cancellationToken = default);
    Task Delete(Guid userId, Guid playlistId, Guid songId, CancellationToken cancellationToken = default);
}