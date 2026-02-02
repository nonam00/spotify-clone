using Microsoft.Extensions.Logging;

using Domain.Common;
using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Shared.Models;
using Application.Users.Errors;
using Application.Users.Interfaces;
using Domain.Models;

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
            _logger.LogInformation(
                "Anonymous user tried to login with non-relevant refresh token {RefreshToken}",
                request.RefreshToken); 
            return Result<TokenPair>.Failure(RefreshTokenErrors.RelevantNotFound);
        }
        
        if (!user.IsActive)
        {
            _logger.LogError(
                "Non-active user {UserId} tried to login with non-relevant refresh token {RefreshToken}.",
                user.Id, request.RefreshToken);
            return Result<TokenPair>.Failure(UserDomainErrors.NotActive);
        }

        var updateRefreshTokenResult = user.UpdateRefreshToken(request.RefreshToken);
        if (updateRefreshTokenResult.IsFailure)
        {
            _logger.LogError(
                "Anonymous user tried to login with refresh token {RefreshToken}" +
                " but domain error occurred: {DomainErrorDescription}",
                request.RefreshToken, updateRefreshTokenResult.Error.Description);
            return Result<TokenPair>.Failure(updateRefreshTokenResult.Error);
        }
        
        var accessToken = _jwtProvider.GenerateUserToken(user);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<TokenPair>.Success(new TokenPair(accessToken, updateRefreshTokenResult.Value.Token));
    }
}