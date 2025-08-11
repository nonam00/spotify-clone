using Microsoft.EntityFrameworkCore;

using Domain;
using Application.Users.Interfaces;
using Application.Users.Models;

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

    public async Task<UserInfo> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw new Exception("Invalid current user id");

        var userVm = ToVm(user);
        return userVm;
    }

    public async Task<bool> CheckIfExists(string email, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken);

        return result;
    }

    private static UserInfo ToVm(User user)
    {
        return new UserInfo
        {
            Email = user.Email,
            FullName = user.FullName
        };
    }
}