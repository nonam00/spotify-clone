using Microsoft.EntityFrameworkCore;

using Domain;
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

    public async Task<SongVm> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var song = await _dbContext.Songs
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new Exception("Song not found");
        
        var songVm = ToVm(song);
        
        return songVm;
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
            .Take(100)
            .ToListAsync(cancellationToken);

        return songs;
    }

    public async Task<List<SongVm>> GetListByPlaylistId(Guid playlistId, CancellationToken cancellationToken = default)
    {
        var songs = await _dbContext.PlaylistSongs
            .AsNoTracking()
            .Where(ps => ps.PlaylistId == playlistId)
            .OrderByDescending(ps => ps.CreatedAt)
            .Select(ps => ToVm(ps.Song))
            .ToListAsync(cancellationToken);

        return songs;
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

    public async Task Add(Song song, CancellationToken cancellationToken = default)
    {
        await _dbContext.Songs.AddAsync(song, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static SongVm ToVm(Song song)
    {
        return new SongVm
        {
            Id = song.Id,
            Title = song.Title,
            Author = song.Author,
            SongPath = song.SongPath,
            ImagePath = song.ImagePath
        };
    }
}