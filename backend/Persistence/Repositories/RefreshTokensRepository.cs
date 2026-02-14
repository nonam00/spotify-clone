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

    public async Task<List<RefreshToken>> GetExpiredList(CancellationToken cancellationToken = default)
    {
        var expiredTokens = await _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(rf => rf.Expires < DateTime.UtcNow)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return expiredTokens;
    }

    public void DeleteRange(IEnumerable<RefreshToken> refreshTokens) =>
        _dbContext.RefreshTokens.RemoveRange(refreshTokens);
}