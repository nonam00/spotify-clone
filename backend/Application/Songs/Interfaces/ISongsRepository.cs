using Domain.Models;
using Application.Songs.Enums;
using Application.Songs.Models;

namespace Application.Songs.Interfaces;

public interface ISongsRepository
{
    Task<Song?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<List<Song>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken = default);
    Task<List<Song>> GetMarkedForDeletion(CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetList(CancellationToken cancellationToken = default);
    Task<List<SongVm>> TakeNewestList(int count = 100, CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetListByPlaylistId(Guid playlistId, CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetSearchList(string searchString, SearchCriteria searchCriteria,
        CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetLikedByUserId(Guid userId, CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetLikedByUserIdExcludeInPlaylist(Guid userId, Guid playlistId,
        CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetSearchLikedByUserIdExcludeInPlaylist(Guid userId, Guid playlistId, string searchString,
        CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetUnpublishedList(CancellationToken cancellationToken = default);
    Task<List<SongVm>> GetUploadedByUserId(Guid userId, CancellationToken cancellationToken = default);
    void Update(Song song);
    void UpdateRange(IEnumerable<Song> songs);
    void DeleteRange(IEnumerable<Song> songs);
}