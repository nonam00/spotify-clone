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

    public async Task<PlaylistVm> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var playlist = await _dbContext.Playlists
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken)
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

    public async Task Delete(Guid playlistId, Guid userId, CancellationToken cancellationToken = default)
    {
        var deletedRows = await _dbContext.Playlists
            .Where(p => p.UserId == userId && p.Id == playlistId)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedRows != 1)
        {
            throw new Exception("Playlist with such ID doesn't exist");
        }
    }

    public async Task Update(Guid playlistId, Guid userId, string title, string? description, string? imagePath,
        CancellationToken cancellationToken = default)
    {
        var updatedRows = await _dbContext.Playlists
            .Where(p => p.UserId == userId && p.Id == playlistId)
            .ExecuteUpdateAsync(p => p
                    .SetProperty(u => u.Title, title)
                    .SetProperty(u => u.Description, description != "" ? description : null)
                    .SetProperty(u => u.ImagePath, u => imagePath != "" ? imagePath : u.ImagePath)
                    .SetProperty(u => u.CreatedAt, DateTime.UtcNow),
                cancellationToken);

        if (updatedRows != 1)
        {
            throw new Exception("Playlist with such ID doesn't exist");
        }
    }

    private static PlaylistVm ToVm(Playlist playlist)
    {
        return new PlaylistVm
        {
            Id = playlist.Id,
            Title = playlist.Title,
            Description = playlist.Description,
            ImagePath = playlist.ImagePath
        };
    }
}