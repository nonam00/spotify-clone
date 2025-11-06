using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Exceptions;
using Application.Shared.Messaging;
using Application.Users.Interfaces;
using Application.Users.Models;

namespace Application.Users.Queries.LoginByRefreshToken;

public class LoginByRefreshTokenQueryHandler : IQueryHandler<LoginByRefreshTokenQuery, TokenPair>
{
    private readonly IRefreshTokensRepository _refreshTokensRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginByRefreshTokenQueryHandler> _logger;

    public LoginByRefreshTokenQueryHandler(
        IRefreshTokensRepository refreshTokensRepository,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork,
        ILogger<LoginByRefreshTokenQueryHandler> logger)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TokenPair> Handle(LoginByRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokensRepository.GetByValueWithUser(request.RefreshToken, cancellationToken);
        
        if (refreshToken is null || refreshToken.Expires < DateTime.UtcNow)
        {
            _logger.LogInformation("Anonymous user tried to login with non-relevant refresh token");
            throw new LoginException("The refresh token has expired or does not exist.");
        }
        
        var accessToken = _jwtProvider.GenerateToken(refreshToken.User);
        
        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        
        refreshToken.UpdateToken(refreshTokenValue, DateTime.UtcNow.AddDays(14));
        
        _refreshTokensRepository.Update(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new TokenPair(accessToken, refreshTokenValue);
    }
}