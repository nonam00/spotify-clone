using Microsoft.EntityFrameworkCore;

using Domain.Models;
using Application.Users.Interfaces;

namespace Persistence.Repositories;

public sealed class RefreshTokensRepository : IRefreshTokensRepository
{
    private readonly AppDbContext _dbContext;
    
    public RefreshTokensRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<RefreshToken>> GetExpiredList(CancellationToken cancellationToken = default)
    {
        return _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(rf => rf.Expires < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public void DeleteRange(IEnumerable<RefreshToken> refreshTokens) =>
        _dbContext.RefreshTokens.RemoveRange(refreshTokens);
}