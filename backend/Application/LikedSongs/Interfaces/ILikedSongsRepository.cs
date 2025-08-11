using Domain;
using Application.LikedSongs.Models;

namespace Application.LikedSongs.Interfaces;

public interface ILikedSongsRepository
{
    Task<List<LikedSongVm>> GetList(Guid userId, CancellationToken cancellationToken = default);
    Task<List<LikedSongVm>> GetListForPlaylist(Guid userId, Guid playlistId, CancellationToken cancellationToken = default);
    Task<List<LikedSongVm>> GetSearchListForPlaylist(Guid userId, string searchString, Guid playlistId,
        CancellationToken cancellationToken = default);
    Task<bool> CheckIfExists(Guid userId, Guid songId, CancellationToken cancellationToken = default);
    Task Add(LikedSong likedSong, CancellationToken cancellationToken = default);
    Task Delete(Guid userId, Guid songId, CancellationToken cancellationToken = default);
}