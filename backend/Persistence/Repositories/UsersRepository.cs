using Application.Users.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SongsDbContext _dbContext;

    public UserRepository(SongsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateUser(User user, CancellationToken cancellationToken = default)
    {
        
    }

    public async Task<User> GetUser(CancellationToken cancellationToken = default)
    {
        
    }

    public async Task<bool> CheckIfExists(string email, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken);

        return result;
    }
}