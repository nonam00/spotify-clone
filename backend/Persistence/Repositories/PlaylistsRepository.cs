using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Application.Playlists.Interfaces;
using Application.Playlists.Models;

namespace Persistence.Repositories;

public sealed class PlaylistsRepository : IPlaylistsRepository
{
    private readonly AppDbContext _dbContext;
    
    public PlaylistsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Playlist?> GetById(Guid playlistId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Playlists
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken); 
    }

    public Task<Playlist?> GetByIdWithSongs(Guid playlistId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Playlists
            .Include(p => p.PlaylistSongs)
            .SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken);
    }

    public Task<List<PlaylistVm>> GetList(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Playlists
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.UpdatedAt)
            .Select(ToVmExpression)
            .ToListAsync(cancellationToken);
    }

    public Task<List<PlaylistVm>> TakeList(Guid userId, int count, CancellationToken cancellationToken = default)
    {
        return _dbContext.Playlists
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.UpdatedAt)
            .Select(ToVmExpression)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public Task<List<PlaylistVm>> GetListWithoutSong(
        Guid userId, Guid songId, CancellationToken cancellationToken = default)
    {
        var playlistWithSong = _dbContext.PlaylistSongs
            .AsNoTracking()
            .Where(ps => ps.SongId == songId)
            .Select(ps => ps.PlaylistId);

        return _dbContext.Playlists
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Where(p => !playlistWithSong.Contains(p.Id))
            .Select(ToVmExpression)
            .ToListAsync(cancellationToken);
    }

    public void Update(Playlist playlist) => _dbContext.Playlists.Update(playlist);

    private static readonly Expression<Func<Playlist, PlaylistVm>> ToVmExpression = playlist =>
        new PlaylistVm(playlist.Id, playlist.Title, playlist.Description, playlist.ImagePath.Value);
}