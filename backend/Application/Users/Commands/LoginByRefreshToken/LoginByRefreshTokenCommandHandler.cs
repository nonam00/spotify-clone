using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Shared.Models;
using Application.Users.Errors;
using Application.Users.Interfaces;
using Domain.Common;

namespace Application.Users.Commands.LoginByRefreshToken;

public class LoginByRefreshTokenCommandHandler : IQueryHandler<LoginByRefreshTokenCommand, Result<TokenPair>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginByRefreshTokenCommandHandler> _logger;

    public LoginByRefreshTokenCommandHandler(
        IUsersRepository usersRepository,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork,
        ILogger<LoginByRefreshTokenCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TokenPair>> Handle(LoginByRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByRefreshTokenValue(request.RefreshToken, cancellationToken);
        
        if (user is null)
        {
            _logger.LogInformation("Anonymous user tried to login with non-relevant refresh token"); 
            return Result<TokenPair>.Failure(RefreshTokenErrors.RelevantNotFound);
        }
        
        var accessToken = _jwtProvider.GenerateUserToken(user);
        var refreshToken = user.UpdateRefreshToken(request.RefreshToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<TokenPair>.Success(new TokenPair(accessToken, refreshToken!.Token));
    }
}