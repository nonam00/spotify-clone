using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Application.Songs.Enums;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Persistence.Repositories;

public class SongsRepository : ISongsRepository
{
    private readonly SongsDbContext _dbContext;

    public SongsRepository(SongsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Song?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var song = await _dbContext.Songs
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
        
        return song;
    }

    public async Task<List<SongVm>> GetList(CancellationToken cancellationToken = default)
    {
        var songs = await _dbContext.Songs
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => ToVm(s))
            .ToListAsync(cancellationToken);
        
        return songs;
    }

    public async Task<List<SongVm>> TakeNewestList(int count = 100, CancellationToken cancellationToken = default)
    {
        var songs = await _dbContext.Songs
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => ToVm(s))
            .Take(count)
            .ToListAsync(cancellationToken);

        return songs;
    }

    public async Task<List<SongVm>> GetListByPlaylistId(Guid playlistId, CancellationToken cancellationToken = default)
    {
        var songsInPlaylist = await _dbContext.PlaylistSongs
            .AsNoTracking()
            .Where(ps => ps.PlaylistId == playlistId)
            .OrderByDescending(ps => ps.CreatedAt)
            .Include(ps => ps.Song)
            .Select(ps => ToVm(ps.Song)) 
            .ToListAsync(cancellationToken);

        return songsInPlaylist;
    }

    public async Task<List<SongVm>> GetSearchList(string searchString, SearchCriteria searchCriteria,
        CancellationToken cancellationToken = default)
    {
        var songs = _dbContext.Songs.AsNoTracking();

        switch (searchCriteria)
        {
            case SearchCriteria.Any:
                songs = songs
                    .Where(song =>
                        EF.Functions.TrigramsSimilarity(song.Title, searchString) > 0.1 ||
                        EF.Functions.TrigramsSimilarity(song.Author, searchString) > 0.1)
                    .OrderBy(song =>EF.Functions.TrigramsSimilarityDistance(song.Title, searchString))
                    .ThenBy(song => EF.Functions.TrigramsSimilarityDistance(song.Author, searchString))
                    .ThenByDescending(song => song.CreatedAt);
                break;
            case SearchCriteria.Title:
                songs = songs
                    .Where(song => EF.Functions.TrigramsSimilarity(song.Title, searchString) > 0.1)
                    .OrderBy(song => EF.Functions.TrigramsSimilarityDistance(song.Title, searchString))
                    .ThenByDescending(song => song.CreatedAt);
                break;
            case SearchCriteria.Author:
                songs = songs
                    .Where(song => EF.Functions.TrigramsSimilarity(song.Author, searchString) > 0.1)
                    .OrderBy(song => EF.Functions.TrigramsSimilarityDistance(song.Author, searchString))
                    .ThenByDescending(song => song.CreatedAt);

                break;
            default:
                throw new Exception("Invalid search criteria");
        }
        
        var result = await songs
            .Take(50)
            .Select(s => ToVm(s))
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<List<SongVm>> GetLikedByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        var liked = await _dbContext.LikedSongs
            .AsNoTracking()
            .Where(ls => ls.UserId == userId)
            .OrderByDescending(ls => ls.CreatedAt)
            .Include(ls => ls.Song)
            .Select(ls => ToVm(ls.Song))
            .ToListAsync(cancellationToken);
            
        return liked;
    }

    public async Task<List<SongVm>> GetSearchLikedByUserId(Guid userId, string searchString, CancellationToken cancellationToken = default)
    {
        var liked = await _dbContext.LikedSongs
            .AsNoTracking()
            .Where(ls => ls.UserId == userId)
            .Include(ls => ls.Song)
            .Where(ls =>
                EF.Functions.TrigramsSimilarity(ls.Song.Title, searchString) > 0.1 ||
                EF.Functions.TrigramsSimilarity(ls.Song.Author, searchString) > 0.1)
            .OrderBy(ls => EF.Functions.TrigramsSimilarityDistance(ls.Song.Title, searchString))
            .ThenBy(ls => EF.Functions.TrigramsSimilarityDistance(ls.Song.Author, searchString))
            .ThenByDescending(ls => ls.CreatedAt)
            .Select(ls => ToVm(ls.Song))
            .ToListAsync(cancellationToken);

        return liked;
    }

    public async Task Add(Song song, CancellationToken cancellationToken = default)
    {
        await _dbContext.Songs.AddAsync(song, cancellationToken);
    }

    private static SongVm ToVm(Song song) =>
        new(
            Id: song.Id, 
            Title: song.Title,
            Author: song.Author,
            SongPath: song.SongPath,
            ImagePath: song.ImagePath);
}