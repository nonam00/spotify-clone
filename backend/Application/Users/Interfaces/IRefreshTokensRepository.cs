using Domain;

namespace Application.Users.Interfaces;

public interface IRefreshTokensRepository
{
    Task Add(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshToken> GetByValue(string token, CancellationToken cancellationToken = default);
    Task<RefreshToken> GetRelevantByValue(string token, CancellationToken cancellationToken = default);
    Task Update(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task Delete(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task DeleteExpired(CancellationToken cancellationToken = default);
}