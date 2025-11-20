using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Shared.Models;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Queries.LoginByCredentials;

public class LoginByCredentialsQueryHandler : IQueryHandler<LoginByCredentialsQuery, Result<TokenPair>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginByCredentialsQueryHandler> _logger;
    
    public LoginByCredentialsQueryHandler(IUsersRepository usersRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork, ILogger<LoginByCredentialsQueryHandler> logger)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TokenPair>> Handle(LoginByCredentialsQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByEmailWithRefreshTokens(request.Email, cancellationToken);

        if (user == null)
        {
            return Result<TokenPair>.Failure(UserErrors.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            _logger.LogInformation("Non active user {userId} with tried to login.", user.Id);
            return Result<TokenPair>.Failure(UserErrors.AlreadyExistButNotActive);
        }
        
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogInformation("Invalid login attempt by user {userId}.", user.Id);
            return Result<TokenPair>.Failure(UserErrors.InvalidCredentials);
        }

        var accessToken = _jwtProvider.GenerateUserToken(user);

        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        user.AddRefreshToken(refreshTokenValue, DateTime.UtcNow.AddDays(14));
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Generated token pair for user {userId}", user.Id);

        return Result<TokenPair>.Success(new TokenPair(accessToken, refreshTokenValue));
    }
}