using Microsoft.EntityFrameworkCore;

using Domain;
using Application.LikedSongs.Interfaces;
using Application.LikedSongs.Models;

namespace Persistence.Repositories;

public class LikedSongsRepository : ILikedSongsRepository
{
    private readonly SongsDbContext _dbContext;

    public LikedSongsRepository(SongsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<LikedSongVm>> GetList(Guid userId, CancellationToken cancellationToken = default)
    {
        var liked = await _dbContext.LikedSongs
            .AsNoTracking()
            .Where(liked => liked.UserId == userId)
            .OrderByDescending(liked => liked.CreatedAt)
            .Select(liked => ToVm(liked))
            .ToListAsync(cancellationToken);

        return liked;
    }

    public async Task<List<LikedSongVm>> GetListForPlaylist(Guid userId, Guid playlistId,
        CancellationToken cancellationToken = default)
    {
        var liked = await _dbContext.LikedSongs
            .AsNoTracking()
            .Where(l => l.UserId == userId &&
                        !_dbContext.PlaylistSongs.Any(ps => ps.PlaylistId == playlistId &&
                                                            ps.SongId == l.SongId))
            .OrderByDescending(l => l.CreatedAt)
            .Select(liked => ToVm(liked))
            .ToListAsync(cancellationToken);
        
        return liked;
    }

    public async Task<List<LikedSongVm>> GetSearchListForPlaylist(Guid userId, string searchString, Guid playlistId,
        CancellationToken cancellationToken = default)
    {
        var liked = await _dbContext.LikedSongs
            .AsNoTracking()
            .Where(l => l.UserId == userId &&
                        !_dbContext.PlaylistSongs.Any(ps => ps.PlaylistId == playlistId && ps.SongId == l.SongId) &&
                        (EF.Functions.TrigramsSimilarity(l.Song.Title, searchString) > 0.1 ||
                        EF.Functions.TrigramsSimilarity(l.Song.Author, searchString) > 0.1))
            .OrderBy(l => EF.Functions.TrigramsSimilarityDistance(l.Song.Title, searchString))
            .ThenBy(l => EF.Functions.TrigramsSimilarityDistance(l.Song.Author, searchString))
            .ThenByDescending(song => song.CreatedAt)
            .Take(50)
            .Select(liked => ToVm(liked))
            .ToListAsync(cancellationToken);

        return liked;
    }

    public async Task<bool> CheckIfExists(Guid userId, Guid songId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LikedSongs
            .AsNoTracking()  
            .AnyAsync(l => l.SongId == songId && l.UserId == userId, cancellationToken);    
    }

    public async Task Add(LikedSong likedSong, CancellationToken cancellationToken = default)
    {
        await _dbContext.LikedSongs.AddAsync(likedSong, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);   
    }

    public async Task Delete(Guid userId, Guid songId, CancellationToken cancellationToken = default)
    {
        var deletedRows = await _dbContext.LikedSongs
            .Where(l => l.UserId == userId && l.SongId == songId)
            .ExecuteDeleteAsync(cancellationToken);
            
        if (deletedRows != 1)
        {
            throw new Exception(nameof(LikedSong));
        }
    }

    private static LikedSongVm ToVm(LikedSong liked)
    {
        return new LikedSongVm
        {
            Id = liked.Song.Id,
            Title = liked.Song.Title,
            Author = liked.Song.Author,
            SongPath = liked.Song.SongPath,
            ImagePath = liked.Song.ImagePath
        };
    } 
}