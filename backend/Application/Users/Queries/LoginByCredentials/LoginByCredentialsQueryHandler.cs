using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Exceptions;
using Application.Shared.Messaging;
using Application.Users.Interfaces;
using Application.Users.Models;

namespace Application.Users.Queries.LoginByCredentials;

public class LoginByCredentialsQueryHandler : IQueryHandler<LoginByCredentialsQuery, TokenPair>
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

    public async Task<TokenPair> Handle(LoginByCredentialsQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByEmailWithRefreshTokens(request.Email, cancellationToken)
            ?? throw new LoginException("Invalid email or password. Please try again.");

        if (!user.IsActive)
        {
            _logger.LogInformation("Non active user {userId} with tried to login.", user.Id);
            throw new LoginException("Account is not activated");
        }
        
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogInformation("Invalid login attempt by user {userId}.", user.Id);
            throw new LoginException("Invalid email or password. Please try again.");
        }

        var accessToken = _jwtProvider.GenerateToken(user);

        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        user.AddRefreshToken(refreshTokenValue, DateTime.UtcNow.AddDays(14));
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Generated token pair for user {userId}", user.Id);

        return new TokenPair(accessToken, refreshTokenValue);
    }
}