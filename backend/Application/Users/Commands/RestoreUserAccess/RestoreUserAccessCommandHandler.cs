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
    private readonly ICodesRepository _codesRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreUserAccessCommandHandler> _logger;

    public RestoreUserAccessCommandHandler(
        IUsersRepository usersRepository,
        ICodesRepository codesRepository,
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<RestoreUserAccessCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _codesRepository = codesRepository;
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TokenPair>> Handle(RestoreUserAccessCommand request, CancellationToken cancellationToken)
    {
        var restoreCode = await _codesRepository.GetRestoreCode(request.Email).ConfigureAwait(false);
        if (request.RestoreCode != restoreCode)
        {
            _logger.LogInformation(
                "Someone tried to restore access to user account with email {Email} using invalid restore code.", 
                request.Email);
            return Result<TokenPair>.Failure(UserErrors.InvalidVerificationCode);
        }
        
        var user = await _usersRepository.GetByEmail(request.Email, cancellationToken).ConfigureAwait(false);

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
        
        var hash = _passwordHasher.Generate("12345678");
        var passwordHash = new PasswordHash(hash);
        
        user.ChangePassword(passwordHash);
        _usersRepository.Update(user);
        
        var addRefreshTokenResult = user.AddRefreshToken();
        if (addRefreshTokenResult.IsFailure)
        {
            _logger.LogError(
                "User {UserId} tried to restore access to account using restore code" +
                " but domain error occurred when creating refresh token:\n{DomainErrorResult}",
                user.Id, addRefreshTokenResult.Error.Description);
            return Result<TokenPair>.Failure(addRefreshTokenResult.Error);
        }
        
        var accessToken = _jwtProvider.GenerateUserToken(user);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        
        _logger.LogInformation("User {UserId} successfully restored access to account using restore code.", user.Id);
        
        return Result<TokenPair>.Success(new TokenPair(accessToken, addRefreshTokenResult.Value.Token));
    }
}