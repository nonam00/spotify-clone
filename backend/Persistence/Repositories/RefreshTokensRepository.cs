using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Application.Users.Interfaces;

namespace Persistence.Repositories;

public class RefreshTokensRepository : IRefreshTokensRepository
{
    private readonly AppDbContext _dbContext;
    
    public RefreshTokensRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<RefreshToken?> GetByValue(string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == token, cancellationToken);

        return refreshToken;
    }

    public async Task<RefreshToken?> GetByValueWithUser(string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _dbContext.RefreshTokens
            .AsNoTracking()
            .Include(r => r.User)
            .FirstOrDefaultAsync(t => t.Token == token, cancellationToken);

        return refreshToken;
    }

    public async Task<List<RefreshToken>> GetExpiredList(CancellationToken cancellationToken = default)
    {
        var expiredTokens = await _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(rf => rf.Expires < DateTime.UtcNow)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return expiredTokens;
    }

    public void Update(RefreshToken refreshToken) =>
        _dbContext.RefreshTokens.Update(refreshToken);
    

    public void Delete(RefreshToken refreshToken) =>
        _dbContext.RefreshTokens.Remove(refreshToken);
    

    public void DeleteRange(IEnumerable<RefreshToken> refreshTokens) =>
        _dbContext.RefreshTokens.RemoveRange(refreshTokens);
    
}