using Microsoft.Extensions.Logging;

using Domain.Common;
using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Shared.Models;
using Application.Users.Errors;
using Application.Users.Interfaces;

namespace Application.Users.Commands.ActivateUserByConfirmationCode;

public class ActivateUserByConfirmationCodeCommandHandler
    : ICommandHandler<ActivateUserByConfirmationCodeCommand, Result<TokenPair>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ICodesClient _codesClient;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateUserByConfirmationCodeCommandHandler> _logger;
    
    public ActivateUserByConfirmationCodeCommandHandler(
        IUsersRepository usersRepository,
        ICodesClient codesClient,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork,
        ILogger<ActivateUserByConfirmationCodeCommandHandler> logger)
    {
        _codesClient = codesClient;
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _jwtProvider = jwtProvider;
        _logger = logger;
    }

    public async Task<Result<TokenPair>> Handle(
        ActivateUserByConfirmationCodeCommand request, CancellationToken cancellationToken)
    {
        var codeVerificationStatus = await _codesClient
            .VerifyConfirmationCodeAsync(request.Email, request.ConfirmationCode)
            .ConfigureAwait(false);
        
        if (!codeVerificationStatus)
        {
            _logger.LogInformation(
                "Someone tried to activate user account with email {Email} using invalid code {ConfirmationCode}",
                request.Email, request.ConfirmationCode);
            return Result<TokenPair>.Failure(UserErrors.InvalidVerificationCode);
        }

        var user = await _usersRepository.GetByEmail(request.Email, cancellationToken);

        if (user == null)
        {
            _logger.LogError(
                "Someone tried to activate non-existing user account with email {Email} and code {ConfirmationCode}", 
                request.Email, request.ConfirmationCode);
            return Result<TokenPair>.Failure(UserErrors.NotFoundWithEmail);
        }
        
        user.Activate();
        _usersRepository.Update(user);

        var accessToken = _jwtProvider.GenerateUserToken(user);
        var refreshToken = user.AddRefreshToken();       
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<TokenPair>.Success(new TokenPair(accessToken, refreshToken.Token));
    }
}