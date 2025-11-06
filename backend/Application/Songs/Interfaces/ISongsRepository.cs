using Domain.Models;
using Application.Songs.Enums;
using Application.Songs.Models;

namespace Application.Songs.Interfaces;

public interface ISongsRepository
{
    Task<Song?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetList(CancellationToken cancellationToken = default);
    Task<List<SongVm>> TakeNewestList(int count = 100, CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetListByPlaylistId(Guid playlistId, CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetSearchList(string searchString, SearchCriteria searchCriteria,
        CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetLikedByUserId(Guid userId, CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetSearchLikedByUserId(Guid userId, string searchString,
        CancellationToken cancellationToken = default);
    Task Add(Song song, CancellationToken cancellationToken = default);
}