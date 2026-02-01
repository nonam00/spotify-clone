using System.Security.Cryptography;
using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Shared.Models;
using Application.Users.Errors;
using Application.Users.Interfaces;
using Domain.Common;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.RestoreUserAccess;

public class RestoreUserAccessCommandHandler : ICommandHandler<RestoreUserAccessCommand, Result<TokenPair>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ICodesClient _codesClient;
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreUserAccessCommandHandler> _logger;

    public RestoreUserAccessCommandHandler(
        IUsersRepository usersRepository,
        ICodesClient codesClient,
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<RestoreUserAccessCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _codesClient = codesClient;
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TokenPair>> Handle(RestoreUserAccessCommand request, CancellationToken cancellationToken)
    {
        var codeVerificationStatus = await _codesClient
            .VerifyRestoreTokenAsync(request.Email, request.RestoreCode)
            .ConfigureAwait(false);
        
        if (!codeVerificationStatus)
        {
            _logger.LogInformation(
                "Someone tried to restore access to user account with email {email} using invalid code {restoreCode}",
                request.Email, request.RestoreCode);
            return Result<TokenPair>.Failure(UserErrors.InvalidVerificationCode);
        }

        var user = await _usersRepository.GetByEmail(request.Email, cancellationToken);

        if (user == null)
        {
            _logger.LogError(
                "Someone tried to restore access to non-existing user account with email {email} and code {restoreCode}", 
                request.Email, request.RestoreCode);
            return Result<TokenPair>.Failure(UserErrors.NotFoundWithEmail);
        }

        var hash = _passwordHasher.Generate("12345678");
        var passwordHash = new PasswordHash(hash);
        user.ChangePassword(passwordHash);
        _usersRepository.Update(user);

        var accessToken = _jwtProvider.GenerateUserToken(user);

        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        user.AddRefreshToken(refreshTokenValue, DateTime.UtcNow.AddDays(14));
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<TokenPair>.Success(new TokenPair(accessToken, refreshTokenValue));
    }
}