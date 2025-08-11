using Microsoft.EntityFrameworkCore;

using Domain;
using Application.PlaylistSongs.Interfaces;

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
            var playlistExist = await _dbContext.Playlists
                .AsNoTracking()  
                .AnyAsync(p => p.UserId == userId && p.Id == playlistId, cancellationToken);

            if (!playlistExist)
            {
                throw new Exception("Playlist with such ID doesn't exist or doesn't belong to the current user");
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

            var updatedRows = await _dbContext.Playlists
                .Where(p => p.Id == playlistId)
                .ExecuteUpdateAsync(p =>
                        p.SetProperty(u => u.CreatedAt, DateTime.UtcNow),
                    cancellationToken);

            if (updatedRows != 1)
            {
                throw new Exception("Playlist with such ID doesn't exist");
            }

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
            var deletedRows = await _dbContext.PlaylistSongs
                .Where(ps => ps.Playlist.UserId == userId && 
                             ps.PlaylistId == playlistId &&
                             ps.SongId == songId)
                .ExecuteDeleteAsync(cancellationToken);
                
            if (deletedRows != 1) 
            {
                throw new Exception("Relation between playlist and song with such key doesn't exist");
            }

            var updatedRows = await _dbContext.Playlists
                .Where(p => p.UserId == userId && p.Id == playlistId)
                .ExecuteUpdateAsync(p =>
                    p.SetProperty(u => u.CreatedAt, DateTime.UtcNow), cancellationToken);

            if (updatedRows != 1)
            {
                throw new Exception("Playlist with such ID doesn't exist");
            }
                
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
