using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Application.Users.Interfaces;
using Application.Users.Models;

namespace Persistence.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly AppDbContext _dbContext;

    public UsersRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }
    
    public async Task<User?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
        return user;
    }

    public async Task<User?> GetByIdWithUploadedSongs(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .Include(u => u.UploadedSongs)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
        
        return user;
    }

    public async Task<User?> GetByIdWithLikedSongs(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserLikedSongs)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
        
        return user;
    }

    public async Task<User?> GetByIdWithPlaylists(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .Include(u => u.Playlists)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
        
        return user;
    }

    public async Task<User?> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
        
        return user;
    }

    public async Task<User?> GetByEmailWithRefreshTokens(string email, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
        
        return user;   
    }

    public async Task<bool> CheckIfSongLiked(Guid userId, Guid songId, CancellationToken cancellationToken = default)
    {
        var isLiked = await _dbContext.LikedSongs
            .AsNoTracking()
            .AnyAsync(l => l.UserId == userId && l.SongId == songId, cancellationToken)
            .ConfigureAwait(false);
        
        return isLiked;
    }

    public async Task<List<User>> GetNonActiveList(CancellationToken cancellationToken = default)
    {
        var nonActiveUsers = await _dbContext.Users
            .AsNoTracking()
            .Where(u => !u.IsActive && u.CreatedAt.AddHours(1) < DateTime.UtcNow)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
            
        return nonActiveUsers;
    }
    
    public async Task<List<UserVm>> GetListVm(CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users
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
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return users;
    }

    public void Update(User user) => _dbContext.Update(user);
    
    public void DeleteRange(IEnumerable<User> users) =>
        _dbContext.Users.RemoveRange(users);
}