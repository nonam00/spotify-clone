using Microsoft.EntityFrameworkCore;

using Domain;
using Application.Playlists.Interfaces;
using Application.Playlists.Models;

namespace Persistence.Repositories;

public class PlaylistsRepository : IPlaylistsRepository
{
    private readonly SongsDbContext _dbContext;

    public PlaylistsRepository(SongsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Playlist> GetById(Guid playlistId, Guid userId, CancellationToken cancellationToken = default)
    {
        var playlist = await _dbContext.Playlists
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == playlistId && p.UserId == userId, cancellationToken)
            ?? throw new Exception("Playlist this such ID doesn't exist");
            
        return playlist;
    }

    public async Task<PlaylistVm> GetVmById(Guid playlistId, Guid userId, CancellationToken cancellationToken = default)
    {
        var playlist = await _dbContext.Playlists
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == playlistId && p.UserId == userId, cancellationToken) 
            ?? throw new Exception("Playlist this such ID doesn't exist");

        var playlistVm = ToVm(playlist);
        return playlistVm;
    }

    public async Task<List<PlaylistVm>> GetList(Guid userId, CancellationToken cancellationToken = default)
    {
        var playlists = await _dbContext.Playlists
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => ToVm(p))
            .ToListAsync(cancellationToken);
        
        return playlists;
    }

    public async Task<List<PlaylistVm>> TakeList(Guid userId, int count, CancellationToken cancellationToken = default)
    {
        var playlists = await _dbContext.Playlists
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => ToVm(p))
            .Take(count)
            .ToListAsync(cancellationToken);

        return playlists;
    }

    public async Task<int> GetCount(Guid userId, CancellationToken cancellationToken = default)
    {
        var count = await _dbContext.Playlists
            .AsNoTracking()
            .CountAsync(p => p.UserId == userId, cancellationToken);

        return count;
    }

    public async Task Add(Playlist playlist, CancellationToken cancellationToken = default)
    {
        await _dbContext.Playlists.AddAsync(playlist, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(Playlist playlist, CancellationToken cancellationToken = default)
    {
        _dbContext.Playlists.Remove(playlist);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Playlist playlist, CancellationToken cancellationToken = default)
    {
        _dbContext.Playlists.Update(playlist);
        await _dbContext.SaveChangesAsync(cancellationToken);   
    }

    private static PlaylistVm ToVm(Playlist playlist) =>
        new(playlist.Id, playlist.Title, playlist.Description, playlist.ImagePath);
    
}