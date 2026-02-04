using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Application.Songs.Enums;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Persistence.Repositories;

public class SongsRepository : ISongsRepository
{
    private readonly AppDbContext _dbContext;

    public SongsRepository(AppDbContext dbContext)
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

    public async Task<List<Song>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var songs = await _dbContext.Songs
            .AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return songs;
    }

    public async Task<List<Song>> GetMarkedForDeletion(CancellationToken cancellationToken = default)
    {
        var songs = await _dbContext.Songs
            .AsNoTracking()
            .Where(s => s.MarkedForDeletion)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return songs;
    }

    public async Task<List<SongVm>> GetList(CancellationToken cancellationToken = default)
    {
        var songs = await _dbContext.Songs
            .AsNoTracking()
            .Where(s => s.IsPublished)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => ToVm(s))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return songs;
    }

    public async Task<List<SongVm>> TakeNewestList(int count = 100, CancellationToken cancellationToken = default)
    {
        var songs = await _dbContext.Songs
            .AsNoTracking()
            .Where(s => s.IsPublished)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => ToVm(s))
            .Take(count)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return songs;
    }

    public async Task<List<SongVm>> GetListByPlaylistId(Guid playlistId, CancellationToken cancellationToken = default)
    {
        var songsInPlaylist = await _dbContext.PlaylistSongs
            .AsNoTracking()
            .Where(ps => ps.PlaylistId == playlistId && ps.Song.IsPublished)
            .OrderByDescending(ps => ps.Order)
            .Include(ps => ps.Song)
            .Select(ps => ToVm(ps.Song)) 
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return songsInPlaylist;
    }

    public async Task<List<SongVm>> GetSearchList(
        string searchString, SearchCriteria searchCriteria, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchString) || searchString.Length < 3)
        {
            return [];
        }
        
        // To make search not sensitive to string register
        var lowerSearch = RemoveDiacritics(searchString.Trim().ToLower());

        // Dynamic similarity threshold
        var similarityThreshold = CalculateSimilarityThreshold(searchString.Length);
        
        var songs = _dbContext.Songs
            .AsNoTracking()
            .Where(s => s.IsPublished);

        switch (searchCriteria)
        {
            case SearchCriteria.Any:
                songs = songs
                    .Where(song =>
                        EF.Property<string>(song, "TitleLower").Contains(lowerSearch) || // Fast prefilter
                        EF.Property<string>(song, "AuthorLower").Contains(lowerSearch) ||
                        EF.Functions.TrigramsSimilarity(
                            EF.Property<string>(song, "TitleLower"),
                            lowerSearch)
                            > similarityThreshold ||
                        EF.Functions.TrigramsSimilarity(                            
                            EF.Property<string>(song, "AuthorLower"),
                            lowerSearch)
                            > similarityThreshold)
                    .OrderByDescending(song =>
                        (EF.Property<string>(song, "TitleLower").Contains(lowerSearch) ? 1 : 0) +
                        (EF.Property<string>(song, "AuthorLower").Contains(lowerSearch) ? 1 : 0))
                    .ThenByDescending(song =>
                        Math.Max(
                            EF.Functions.TrigramsSimilarity(
                                EF.Property<string>(song, "TitleLower"),
                                lowerSearch),
                            EF.Functions.TrigramsSimilarity(
                                EF.Property<string>(song, "AuthorLower"),
                                lowerSearch)))
                    .ThenByDescending(song => song.CreatedAt);
                break;
            case SearchCriteria.Title:
                songs = songs
                    .Where(song =>
                        EF.Property<string>(song, "TitleLower").Contains(lowerSearch) || // Fast prefilter
                        EF.Functions.TrigramsSimilarity(
                            EF.Property<string>(song, "TitleLower"),
                            lowerSearch)
                            > similarityThreshold)
                    .OrderByDescending(song =>
                        EF.Property<string>(song, "TitleLower").Contains(lowerSearch) ? 1 : 0)
                    .ThenByDescending(song =>
                        EF.Functions.TrigramsSimilarity(
                            EF.Property<string>(song, "TitleLower"),
                            lowerSearch))
                    .ThenByDescending(song => song.CreatedAt);
                break;
            case SearchCriteria.Author:
                songs = songs
                    .Where(song =>
                        EF.Property<string>(song, "AuthorLower").Contains(lowerSearch) || // Fast prefilter
                        EF.Functions.TrigramsSimilarity(
                            EF.Property<string>(song, "AuthorLower"),
                            lowerSearch)
                            > similarityThreshold)
                    .OrderByDescending(song =>
                        EF.Property<string>(song, "AuthorLower").Contains(lowerSearch) ? 1 : 0)
                    .ThenByDescending(song =>
                        EF.Functions.TrigramsSimilarity(
                            EF.Property<string>(song, "AuthorLower"),
                            lowerSearch))         
                    .ThenByDescending(song => song.CreatedAt);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(searchCriteria), searchCriteria, "Invalid criteria");
        }
        
        var result = await songs
            .Take(50)
            .Select(s => ToVm(s))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    public async Task<List<SongVm>> GetLikedByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        var liked = await _dbContext.LikedSongs
            .AsNoTracking()
            .Where(ls => ls.UserId == userId && ls.Song.IsPublished)
            .OrderByDescending(ls => ls.CreatedAt)
            .Include(ls => ls.Song)
            .Select(ls => ToVm(ls.Song))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
            
        return liked;
    }

    public async Task<List<SongVm>> GetLikedByUserIdExcludeInPlaylist(
        Guid userId, Guid playlistId, CancellationToken cancellationToken = default)
    {
        var songsInPlaylist = _dbContext.PlaylistSongs
            .AsNoTracking()
            .Where(ps => ps.PlaylistId == playlistId && ps.Song.IsPublished);
        
        var result = await _dbContext.LikedSongs
            .AsNoTracking()
            .Include(ls => ls.Song)
            .Where(ls => ls.UserId == userId && !songsInPlaylist.Any(ps => ps.SongId == ls.SongId))
            .OrderByDescending(ls => ls.CreatedAt)
            .Select(ls => ToVm(ls.Song))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return result;
    }

    public async Task<List<SongVm>> GetSearchLikedByUserIdExcludeInPlaylist(
        Guid userId, Guid playlistId, string searchString, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchString) || searchString.Length < 2)
        {
            return [];
        }
        
        // To make search not sensitive to string register
        var lowerSearch = RemoveDiacritics(searchString.Trim().ToLower());

        // Dynamic similarity threshold
        var similarityThreshold = CalculateSimilarityThreshold(searchString.Length);
        
        var songsInPlaylist = _dbContext.PlaylistSongs
            .AsNoTracking()
            .Where(ps => ps.PlaylistId == playlistId && ps.Song.IsPublished)
            .Select(ps => ps.SongId);
        
        var result = await _dbContext.LikedSongs
            .AsNoTracking()
            .Include(ls => ls.Song)
            .Where(ls =>
                ls.UserId == userId &&
                ls.Song.IsPublished &&
                !songsInPlaylist.Contains(ls.SongId) &&
                (EF.Property<string>(ls.Song, "TitleLower").Contains(lowerSearch) || // Fast prefilter
                 EF.Property<string>(ls.Song, "AuthorLower").Contains(lowerSearch) ||
                 EF.Functions.TrigramsSimilarity(
                     EF.Property<string>(ls.Song, "TitleLower"),
                     lowerSearch)
                    > similarityThreshold ||
                 EF.Functions.TrigramsSimilarity(
                     EF.Property<string>(ls.Song, "AuthorLower"),
                     lowerSearch)
                    > similarityThreshold))
            .OrderByDescending(ls =>
                (EF.Property<string>(ls.Song, "TitleLower").Contains(lowerSearch) ? 1 : 0) +
                (EF.Property<string>(ls.Song, "AuthorLower").Contains(lowerSearch) ? 1 : 0))
            .ThenByDescending(ls =>
                Math.Max(
                    EF.Functions.TrigramsSimilarity(
                        EF.Property<string>(ls.Song, "TitleLower"),
                        lowerSearch),
                    EF.Functions.TrigramsSimilarity(
                        EF.Property<string>(ls.Song, "AuthorLower"),
                        lowerSearch)))
            .ThenByDescending(ls => ls.CreatedAt)
            .Select(ls => ToVm(ls.Song))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return result;
    }

    public async Task<List<SongVm>> GetUnpublishedList(CancellationToken cancellationToken = default)
    {
        var songs = await _dbContext.Songs
            .AsNoTracking()
            .Where(s => !s.IsPublished && !s.MarkedForDeletion)
            .Select(s => ToVm(s))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return songs;
    }
    
    public async Task<List<SongVm>> GetUploadedByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        var songs = await _dbContext.Songs
            .AsNoTracking()
            .Where(s => s.UploaderId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => ToVm(s))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return songs;
    }

    public async Task Add(Song song, CancellationToken cancellationToken = default)
    {
        await _dbContext.Songs.AddAsync(song, cancellationToken);
    }

    public void Update(Song song) => _dbContext.Songs.Update(song);
    public void UpdateRange(IEnumerable<Song> songs) => _dbContext.Songs.UpdateRange(songs);

    public void Delete(Song song) => _dbContext.Songs.Remove(song);

    public void DeleteRange(IEnumerable<Song> songs) => _dbContext.Songs.RemoveRange(songs);

    private static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }
        
        var formD = text.Normalize(NormalizationForm.FormD);
        var chars = formD
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray();
        
        return new string(chars);
    }
    
    private static double CalculateSimilarityThreshold(int length) => length > 6 ? 0.2 : 0.1;
    
    private static SongVm ToVm(Song song) =>
        new(
            Id: song.Id, 
            Title: song.Title,
            Author: song.Author,
            SongPath: song.SongPath,
            ImagePath: song.ImagePath,
            IsPublished: song.IsPublished,
            CreatedAt: song.CreatedAt);
}