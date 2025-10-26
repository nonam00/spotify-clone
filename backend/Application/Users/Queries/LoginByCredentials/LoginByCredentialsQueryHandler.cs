using System.Security.Cryptography;
using Application.Shared.Exceptions;
using Application.Shared.Messaging;

using Domain;
using Application.Users.Interfaces;
using Application.Users.Models;

namespace Application.Users.Queries.LoginByCredentials;

public class LoginByCredentialsQueryHandler : IQueryHandler<LoginByCredentialsQuery, TokenPair>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokensRepository _refreshTokensRepository;

    public LoginByCredentialsQueryHandler(IUsersRepository usersRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        IRefreshTokensRepository refreshTokensRepository)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _refreshTokensRepository = refreshTokensRepository;
    }

    public async Task<TokenPair> Handle(LoginByCredentialsQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByEmail(request.Email, cancellationToken)
            ?? throw new LoginException("Invalid email or password. Please try again.");

        if (!user.IsActive)
        {
            throw new LoginException("Account is not activated");
        }
        
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new LoginException("Invalid email or password. Please try again.");
        }

        var accessToken = _jwtProvider.GenerateToken(user);

        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenValue,
            Expires = DateTime.UtcNow.AddDays(14),
            CreatedAt = DateTime.UtcNow
        };
        
        await _refreshTokensRepository.Add(refreshToken, cancellationToken);

        return new TokenPair(accessToken, refreshTokenValue);
    }
}