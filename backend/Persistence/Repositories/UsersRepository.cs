using Microsoft.EntityFrameworkCore;

using Domain;
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
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
        
        return user;
    }

    public async Task<User?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
        
        return user;
    }

    public async Task<bool> CheckIfExists(string email, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken);

        return result;
    }
}