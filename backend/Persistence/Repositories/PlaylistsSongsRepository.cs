using Microsoft.EntityFrameworkCore;

using Domain;
using Application.PlaylistSongs.Interfaces;
using Application.Shared.Exceptions;

namespace Persistence.Repositories;

public class PlaylistsSongsRepository : IPlaylistsSongsRepository
{
    private readonly SongsDbContext _dbContext;

    public PlaylistsSongsRepository(SongsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> Create(Guid userId, Guid playlistId, Guid songId,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var playlist = await _dbContext.Playlists
                .AsNoTracking()  
                .SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken)
                ?? throw new Exception("Playlist with such ID doesn't exist");

            if (playlist.UserId != userId)
            {
                throw new OwnershipException("Playlist doesn't belong to current user");
            }
                
            var songExists = await _dbContext.Songs
                .AsNoTracking()
                .AnyAsync(s => s.Id == songId, cancellationToken);

            if (!songExists)
            {
                throw new Exception("Song with such ID doesn't exist");
            }
                
            var ps = new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = songId
            };

            await _dbContext.PlaylistSongs.AddAsync(ps, cancellationToken);

            playlist.CreatedAt = DateTime.UtcNow;
            _dbContext.Playlists.Update(playlist);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return $"{ps.PlaylistId}:{ps.SongId}";
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task Delete(Guid userId, Guid playlistId, Guid songId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            
        try
        {
            var playlist = await _dbContext.Playlists
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken)
                ?? throw new Exception("Playlist with such ID doesn't exist");

            if (playlist.UserId != userId)
            {
                throw new OwnershipException("Playlist doesn't belong to current user");
            }
            
            var ps = await _dbContext.PlaylistSongs
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId, cancellationToken)
                ?? throw new Exception("Relation between this playlist and song don't exist");
            
            _dbContext.PlaylistSongs.Remove(ps);

            playlist.CreatedAt = DateTime.UtcNow;
            _dbContext.Playlists.Update(playlist);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
