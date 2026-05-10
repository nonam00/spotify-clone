using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Application.Users.Interfaces;
using Application.Users.Models;

namespace Persistence.Repositories;

public sealed class UsersRepository : IUsersRepository
{
    private readonly AppDbContext _dbContext;

    public UsersRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask Add(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken).ConfigureAwait(false);
    }
    
    public Task<User?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<User?> GetByIdWithRefreshTokens(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<User?> GetByIdWithUploadedSongs(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .Include(u => u.UploadedSongs)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<User?> GetByIdWithLikedSongs(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .Include(u => u.UserLikedSongs)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<User?> GetByIdWithPlaylists(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .Include(u => u.Playlists)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<User?> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<User?> GetByEmailWithRefreshTokens(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<User?> GetByRefreshTokenValue(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var refreshToken = _dbContext.RefreshTokens
            .FirstOrDefault(rf => rf.Token == refreshTokenValue && rf.Expires >= DateTime.UtcNow);
        
        return _dbContext.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.RefreshTokens.Contains(refreshToken), cancellationToken);
    }

    public Task<bool> CheckIfSongLiked(Guid userId, Guid songId, CancellationToken cancellationToken = default)
    {
        return _dbContext.LikedSongs
            .AsNoTracking()
            .AnyAsync(l => l.UserId == userId && l.SongId == songId, cancellationToken);
    }

    public Task<List<User>> GetNonActiveList(CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .Where(u => !u.IsActive && u.CreatedAt.AddHours(24) < DateTime.UtcNow)
            .Include(u => u.Playlists)
            .ToListAsync(cancellationToken);
    }
    
    public Task<List<UserVm>> GetListVm(CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserVm(
                u.Id,
                u.Email,
                u.FullName ?? "",
                u.IsActive,
                u.CreatedAt,
                u.UploadedSongs.Count
            ))
            .ToListAsync(cancellationToken);
    }

    public void Update(User user) => _dbContext.Update(user);
    
    public void DeleteRange(IEnumerable<User> users) => _dbContext.Users.RemoveRange(users);
}