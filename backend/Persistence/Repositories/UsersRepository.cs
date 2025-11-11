using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Application.Users.Interfaces;

namespace Persistence.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly SongsDbContext _dbContext;

    public UsersRepository(SongsDbContext dbContext)
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

    public async Task<User?> GetByIdWithSongs(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserLikedSongs)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
        
        return user;
    }

    public async Task<User?> GetByIdWithPlaylists(Guid id, CancellationToken cancellationToken = default)
    {
        var user =  await _dbContext.Users
            .Include(u => u.Playlists)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw new Exception("Invalid user id");
        
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
            .Where(u => !u.IsActive && u.CreatedAt.AddHours(1) < DateTime.UtcNow)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
            
        return nonActiveUsers;
    }

    public void Update(User user) => _dbContext.Update(user);
    
    public void DeleteRange(IEnumerable<User> users) =>
        _dbContext.Users.RemoveRange(users);
}