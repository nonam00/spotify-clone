using System.Security.Cryptography;
using Application.Shared.Messaging;

using Application.Users.Interfaces;
using Application.Users.Models;

namespace Application.Users.Queries.LoginByRefreshToken;

public class LoginByRefreshTokenQueryHandler : IQueryHandler<LoginByRefreshTokenQuery, TokenPair>
{
    private readonly IRefreshTokensRepository _refreshTokensRepository;
    private readonly IJwtProvider _jwtProvider;

    public LoginByRefreshTokenQueryHandler(IRefreshTokensRepository refreshTokensRepository, IJwtProvider jwtProvider)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<TokenPair> Handle(LoginByRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokensRepository.GetRelevantByValue(request.RefreshToken, cancellationToken);
        
        var accessToken = _jwtProvider.GenerateToken(refreshToken.User);
        
        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        
        refreshToken.Token = refreshTokenValue;
        refreshToken.Expires = DateTime.UtcNow.AddDays(14);
        
        await _refreshTokensRepository.Update(refreshToken, cancellationToken);
        
        return new TokenPair(accessToken, refreshTokenValue);
    }
}