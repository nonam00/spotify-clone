using Microsoft.EntityFrameworkCore;

using Domain;
using Application.Users.Interfaces;

namespace Persistence.Repositories;

public class RefreshTokensRepository : IRefreshTokensRepository
{
    private readonly SongsDbContext _dbContext;
    
    public RefreshTokensRepository(SongsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken> GetByValue(string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == token, cancellationToken)
            ?? throw new Exception("Refresh token not found");

        return refreshToken;
    }

    public async Task<RefreshToken> GetRelevantByValue(string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _dbContext.RefreshTokens
            .AsNoTracking()
            .Include(r => r.User)
            .FirstOrDefaultAsync(t => t.Token == token, cancellationToken);

        if (refreshToken is null || refreshToken.Expires < DateTime.UtcNow)
        {
            throw new Exception("The refresh token has expired");
        }

        return refreshToken;
    }

    public async Task Update(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _dbContext.RefreshTokens.Update(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _dbContext.RefreshTokens.Remove(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteExpired(CancellationToken cancellationToken = default)
    {
        var expiredTokens = await _dbContext.RefreshTokens
            .Where(rf => rf.Expires < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        _dbContext.RefreshTokens.RemoveRange(expiredTokens);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}