using Microsoft.Extensions.Logging;

using Domain.Common;
using Domain.Errors;
using Domain.ValueObjects;
using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Shared.Models;
using Application.Users.Errors;
using Application.Users.Interfaces;

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
                "Someone tried to restore access to user account with email {Email} using invalid restore code.}",
                request.Email, request.RestoreCode);
            return Result<TokenPair>.Failure(UserErrors.InvalidVerificationCode);
        }

        var user = await _usersRepository.GetByEmail(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogError(
                "Someone tried to restore access to non-existing user account with email {Email} using restore code.", 
                request.Email);
            return Result<TokenPair>.Failure(UserErrors.NotFoundWithEmail);
        }
                
        if (!user.IsActive)
        {
            _logger.LogError(
                "Non-active user {UserId} tried to restore access to account using restore code.",
                user.Id);
            return Result<TokenPair>.Failure(UserDomainErrors.NotActive);
        }
        
        var addRefreshTokenResult = user.AddRefreshToken();
        if (addRefreshTokenResult.IsFailure)
        {
            _logger.LogError(
                "User {UserId} tried to restore access to account using restore code" +
                " but domain error occurred when creating refresh token:\n{DomainErrorResult}",
                user.Id, addRefreshTokenResult.Error.Description);
            return Result<TokenPair>.Failure(addRefreshTokenResult.Error);
        }

        var hash = _passwordHasher.Generate("12345678");
        var passwordHash = new PasswordHash(hash);

        user.ChangePassword(passwordHash);
        _usersRepository.Update(user);

        var accessToken = _jwtProvider.GenerateUserToken(user);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("User {UserId} successfully restored access to account using restore code.", user.Id);
        
        return Result<TokenPair>.Success(new TokenPair(accessToken, addRefreshTokenResult.Value.Token));
    }
}