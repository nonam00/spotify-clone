using Domain.Models;

namespace Application.Users.Interfaces;

public interface IRefreshTokensRepository
{
    Task<RefreshToken?> GetByValue(string token, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetByValueWithUser(string token, CancellationToken cancellationToken = default);
    Task<List<RefreshToken>> GetExpiredList(CancellationToken cancellationToken = default);
    void Update(RefreshToken refreshToken);
    void Delete(RefreshToken refreshToken);
    void DeleteRange(IEnumerable<RefreshToken> refreshTokens);
}