using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Shared.Models;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Queries.LoginByRefreshToken;

public class LoginByRefreshTokenQueryHandler : IQueryHandler<LoginByRefreshTokenQuery, Result<TokenPair>>
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

    public async Task<Result<TokenPair>> Handle(LoginByRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokensRepository.GetByValueWithUser(request.RefreshToken, cancellationToken);
        
        if (refreshToken is null || refreshToken.Expires < DateTime.UtcNow)
        {
            _logger.LogInformation("Anonymous user tried to login with non-relevant refresh token"); 
            return Result<TokenPair>.Failure(RefreshTokenErrors.RelevantNotFound);
        }
        
        var accessToken = _jwtProvider.GenerateUserToken(refreshToken.User);
        
        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        
        refreshToken.UpdateToken(refreshTokenValue, DateTime.UtcNow.AddDays(14));
        
        _refreshTokensRepository.Update(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<TokenPair>.Success(new TokenPair(accessToken, refreshTokenValue));
    }
}