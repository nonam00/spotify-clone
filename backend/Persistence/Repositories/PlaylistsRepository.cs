using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Application.Playlists.Interfaces;
using Application.Playlists.Models;

namespace Persistence.Repositories;

public class PlaylistsRepository : IPlaylistsRepository
{
    private readonly AppDbContext _dbContext;
    
    public PlaylistsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Playlist?> GetById(Guid playlistId, CancellationToken cancellationToken = default)
    {
        var playlist = await _dbContext.Playlists
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken);
        
        return playlist;
    }

    public async Task<Playlist?> GetByIdWithSongs(Guid playlistId, CancellationToken cancellationToken = default)
    {
        var playlist = await _dbContext.Playlists
            .Include(p => p.PlaylistSongs)
            .SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken);
        
        return playlist;
    }

    public async Task<List<PlaylistVm>> GetList(Guid userId, CancellationToken cancellationToken = default)
    {
        var playlists = await _dbContext.Playlists
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.UpdatedAt)
            .Select(p => ToVm(p))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return playlists;
    }

    public async Task<List<PlaylistVm>> TakeList(Guid userId, int count, CancellationToken cancellationToken = default)
    {
        var playlists = await _dbContext.Playlists
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.UpdatedAt)
            .Select(p => ToVm(p))
            .Take(count)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return playlists;
    }

    public void Update(Playlist playlist) =>
        _dbContext.Playlists.Update(playlist);
    

    private static PlaylistVm ToVm(Playlist playlist) =>
        new (playlist.Id, playlist.Title, playlist.Description, playlist.ImagePath.Value);
    
}