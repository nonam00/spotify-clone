using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Shared.Models;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Commands.ActivateUser;

public class ActivateUserCommandHandler : ICommandHandler<ActivateUserCommand, Result<TokenPair>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ICodesClient _codesClient;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateUserCommandHandler> _logger;
    
    public ActivateUserCommandHandler(
        IUsersRepository usersRepository,
        ICodesClient codesClient,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork,
        ILogger<ActivateUserCommandHandler> logger)
    {
        _codesClient = codesClient;
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<TokenPair>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var codeVerificationStatus = await _codesClient
            .VerifyConfirmationCodeAsync(request.Email, request.ConfirmationCode)
            .ConfigureAwait(false);
        
        if (!codeVerificationStatus)
        {
            _logger.LogInformation(
                "Someone tried to activate user account with email {email} using invalid code {confirmationCode}",
                request.Email, request.ConfirmationCode);
            return Result<TokenPair>.Failure(UserErrors.InvalidVerificationCode);
        }

        var user = await _usersRepository.GetByEmail(request.Email, cancellationToken);

        if (user == null)
        {
            _logger.LogError(
                "Someone tried to activate non-existing user account with email {email} and code {confirmationCode}", 
                request.Email, request.ConfirmationCode);
            return Result<TokenPair>.Failure(UserErrors.NotFoundWithEmail);
        }
        
        user.Activate();
        _usersRepository.Update(user);

        var accessToken = _jwtProvider.GenerateUserToken(user);

        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        user.AddRefreshToken(refreshTokenValue, DateTime.UtcNow.AddDays(14));       
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<TokenPair>.Success(new TokenPair(accessToken, refreshTokenValue));
    }
}