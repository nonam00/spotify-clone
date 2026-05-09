using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Domain.ValueObjects;
using Application.Songs.Enums;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Persistence.Repositories;

public sealed class SongsRepository : ISongsRepository
{
    private readonly AppDbContext _dbContext;

    public SongsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Song?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Songs
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public Task<Song?> GetByIdWithLyricsSegments(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Songs
            .Include(s => s.LyricsSegments)
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public Task<List<Song>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        return _dbContext.Songs
            .AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<List<Song>> GetMarkedForDeletion(CancellationToken cancellationToken = default)
    {
        return _dbContext.Songs
            .AsNoTracking()
            .Where(s => s.MarkedForDeletion)
            .ToListAsync(cancellationToken);
    }

    public Task<List<SongVm>> GetList(CancellationToken cancellationToken = default)
    {
        return _dbContext.Songs
            .AsNoTracking()
            .Where(s => s.IsPublished)
            .OrderByDescending(s => s.CreatedAt)
            .Select(ToVmExpression)
            .ToListAsync(cancellationToken);
    }

    public Task<List<SongVm>> TakeNewestList(int count = 100, CancellationToken cancellationToken = default)
    {
        return _dbContext.Songs
            .AsNoTracking()
            .Where(s => s.IsPublished)
            .OrderByDescending(s => s.CreatedAt)
            .Select(ToVmExpression)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public Task<List<SongVm>> GetListByPlaylistId(Guid playlistId, CancellationToken cancellationToken = default)
    {
        return _dbContext.PlaylistSongs
            .AsNoTracking()
            .Where(ps => ps.PlaylistId == playlistId && ps.Song.IsPublished)
            .OrderByDescending(ps => ps.Order)
            .Include(ps => ps.Song)
            .Select(ps => ps.Song)
            .Select(ToVmExpression)
            .ToListAsync(cancellationToken);
    }

    public Task<List<SongVm>> GetSearchList(
        string searchString,
        SearchCriteria searchCriteria,
        bool searchInLyrics = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchString) || searchString.Length < 3)
        {
            throw new ArgumentException("Search string length must be greater than 3", nameof(searchString));
        }

        // To make search not sensitive to string register
        var normalizedSearch = RemoveDiacritics(searchString.Trim().ToLower());

        var baseQuery = _dbContext.Songs
            .AsNoTracking()
            .Where(s => s.IsPublished);

        IQueryable<Song> filteredQuery;
        if (searchInLyrics)
        {
            filteredQuery = SearchQueryFilters.ApplyLyricsFilter(baseQuery, normalizedSearch);
        }
        else
        {
            filteredQuery = searchCriteria switch
            {
                SearchCriteria.Title => SearchQueryFilters.ApplyTitleFilter(baseQuery, normalizedSearch),
                SearchCriteria.Author => SearchQueryFilters.ApplyAuthorFilter(baseQuery, normalizedSearch),
                SearchCriteria.Any => SearchQueryFilters.ApplyAnyFilter(baseQuery, normalizedSearch),
                _ => throw new ArgumentOutOfRangeException(nameof(searchCriteria), searchCriteria,
                    "Invalid criteria")
            };
        }

        return filteredQuery
            .Take(50)
            .Select(ToVmExpression)
            .ToListAsync(cancellationToken);
    }
    
    public Task<List<SongVm>> GetLikedByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.LikedSongs
            .AsNoTracking()
            .Where(ls => ls.UserId == userId && ls.Song.IsPublished)
            .OrderByDescending(ls => ls.CreatedAt)
            .Include(ls => ls.Song)
            .Select(ls => ls.Song)
            .Select(ToVmExpression)
            .ToListAsync(cancellationToken);
    }
    
    public Task<List<SongVm>> GetLikedByUserIdExcludeInPlaylist(
        Guid userId, Guid playlistId, CancellationToken cancellationToken = default)
    {
        var songsInPlaylist = _dbContext.PlaylistSongs
            .AsNoTracking()
            .Where(ps => ps.PlaylistId == playlistId && ps.Song.IsPublished);

        return _dbContext.LikedSongs
            .AsNoTracking()
            .Include(ls => ls.Song)
            .Where(ls => ls.UserId == userId && !songsInPlaylist.Any(ps => ps.SongId == ls.SongId))
            .OrderByDescending(ls => ls.CreatedAt)
            .Select(ls => ls.Song)
            .Select(ToVmExpression)
            .ToListAsync(cancellationToken);
    }

    public Task<List<SongVm>> GetSearchLikedByUserIdExcludeInPlaylist(
        Guid userId, Guid playlistId, string searchString, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchString) || searchString.Length < 3)
        {
            throw new ArgumentException("Search string length must be greater than 3", nameof(searchString));
        }
        
        // To make search not sensitive to string register
        var lowerSearch = RemoveDiacritics(searchString.Trim().ToLower());
        
        var songsInPlaylist = _dbContext.PlaylistSongs
            .AsNoTracking()
            .Where(ps => ps.PlaylistId == playlistId && ps.Song.IsPublished)
            .Select(ps => ps.SongId);
        
        var baseQuery = _dbContext.LikedSongs
            .AsNoTracking()
            .Where(ls => ls.UserId == userId && ls.Song.IsPublished && !songsInPlaylist.Contains(ls.SongId))
            .Select(ls => ls.Song);
            
       var filteredQuery = SearchQueryFilters.ApplyAnyFilter(baseQuery, lowerSearch);

       return filteredQuery
           .Take(50)
           .Select(ToVmExpression)
           .ToListAsync(cancellationToken);
    }

    public Task<List<SongVm>> GetUnpublishedList(CancellationToken cancellationToken = default)
    {
        return _dbContext.Songs
            .AsNoTracking()
            .Where(s => !s.IsPublished && !s.MarkedForDeletion)
            .Select(ToVmExpression)
            .ToListAsync(cancellationToken);
    }
    
    public Task<List<SongVm>> GetUploadedByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Songs
            .AsNoTracking()
            .Where(s => s.UploaderId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(ToVmExpression)
            .ToListAsync(cancellationToken);
    }

    public Task<List<LyricsSegmentData>> GetLyricsBySongId(Guid songId, CancellationToken cancellationToken = default)
    {
        return _dbContext.LyricsSegments
            .AsNoTracking()
            .Where(ls => ls.SongId == songId)
            .Select(ls => ls.LyricsSegmentData)
            .ToListAsync(cancellationToken);
    }

    public void Update(Song song) => _dbContext.Songs.Update(song);
    
    public void UpdateRange(IEnumerable<Song> songs) => _dbContext.Songs.UpdateRange(songs);
    
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
    
    private static readonly Expression<Func<Song, SongVm>> ToVmExpression = song =>
        new SongVm(
            Id: song.Id, 
            Title: song.Title,
            Author: song.Author,
            AudioPath: song.AudioPath,
            ImagePath: song.ImagePath,
            ContainsExplicitContent: song.ContainsExplicitContent,
            IsPublished: song.IsPublished,
            CreatedAt: song.CreatedAt);
}

// Default similarity threshold is 0.3
internal static class SearchQueryFilters
{
    private const double TitleAuthorWeight = 1.2;
    private const double LyricsWeight = 0.9; 
    
    internal static IQueryable<Song> ApplyTitleFilter(IQueryable<Song> query, string search)
    {
        var filtered = query
            .Where(s =>
                EF.Functions.TrigramsAreSimilar(EF.Property<string>(s, "TitleLower"), search))
            .OrderByDescending(s =>
                EF.Functions.TrigramsSimilarity(EF.Property<string>(s, "TitleLower"), search))
            .ThenByDescending(s => s.CreatedAt);

        return filtered;
        
    }

    internal static IQueryable<Song> ApplyAuthorFilter(IQueryable<Song> query, string search)
    {
        var filtered = query
            .Where(s =>
                EF.Functions.TrigramsAreSimilar(EF.Property<string>(s, "AuthorLower"), search))
            .OrderByDescending(s =>
                EF.Functions.TrigramsSimilarity(EF.Property<string>(s, "AuthorLower"), search))
            .ThenByDescending(s => s.CreatedAt);

        return filtered;
    }

    internal static IQueryable<Song> ApplyAnyFilter(IQueryable<Song> query, string search)
    {
        var filtered = query
            .Where(s =>
                EF.Functions.TrigramsAreSimilar(EF.Property<string>(s, "TitleLower"), search) ||
                EF.Functions.TrigramsAreSimilar(EF.Property<string>(s, "AuthorLower"), search)) 
            .OrderByDescending(s =>
                Math.Max(
                    EF.Functions.TrigramsSimilarity(EF.Property<string>(s, "TitleLower"), search),
                    EF.Functions.TrigramsSimilarity(EF.Property<string>(s, "AuthorLower"), search)))
            .ThenByDescending(s => s.CreatedAt);

        return filtered;
    }

    internal static IQueryable<Song> ApplyLyricsFilter(IQueryable<Song> query, string search)
    { 
        var rankedSongs = query
            .Select(s => new
            {
                Song = s,
                
                TitleSim = EF.Functions.TrigramsSimilarity(
                    EF.Property<string>(s, "TitleLower"), search),
                AuthorSim = EF.Functions.TrigramsSimilarity(
                    EF.Property<string>(s, "AuthorLower"), search),
                
                MaxLyricsSim = s.LyricsSegments
                    .Select(l => EF.Functions.TrigramsSimilarity(
                        EF.Property<string>(l, "NormalizedText"), search))
                    .OrderByDescending(sim => sim)
                    .FirstOrDefault()
            })
            .Where(x => 
                EF.Functions.TrigramsAreSimilar(
                    EF.Property<string>(x.Song, "TitleLower"), search) ||
                EF.Functions.TrigramsAreSimilar(
                    EF.Property<string>(x.Song, "AuthorLower"), search) ||
                x.Song.LyricsSegments.Any(l => 
                    EF.Functions.TrigramsAreSimilar(
                        EF.Property<string>(l, "NormalizedText"), search)))
            .Select(x => new
            {
                x.Song,
                Relevance = Math.Max(
                    Math.Max(x.TitleSim, x.AuthorSim) * TitleAuthorWeight,
                    x.MaxLyricsSim * LyricsWeight)
            })
            .OrderByDescending(x => x.Relevance)
            .ThenByDescending(x => x.Song.CreatedAt)
            .Select(x => x.Song);

        return rankedSongs;
    }
}