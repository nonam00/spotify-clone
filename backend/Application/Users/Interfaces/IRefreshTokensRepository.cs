using Domain.Models;

namespace Application.Users.Interfaces;

public interface IRefreshTokensRepository
{
    Task<List<RefreshToken>> GetExpiredList(CancellationToken cancellationToken = default);
    void DeleteRange(IEnumerable<RefreshToken> refreshTokens);
}