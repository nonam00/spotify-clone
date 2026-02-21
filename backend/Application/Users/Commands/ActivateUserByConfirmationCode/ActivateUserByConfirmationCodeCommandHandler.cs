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
    private readonly ICodesRepository _codesRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateUserByConfirmationCodeCommandHandler> _logger;
    
    public ActivateUserByConfirmationCodeCommandHandler(
        IUsersRepository usersRepository,
        ICodesRepository codesRepository,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork,
        ILogger<ActivateUserByConfirmationCodeCommandHandler> logger)
    {
        _codesRepository = codesRepository;
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _jwtProvider = jwtProvider;
        _logger = logger;
    }

    public async Task<Result<TokenPair>> Handle(
        ActivateUserByConfirmationCodeCommand request, CancellationToken cancellationToken)
    {
        var confirmationCode = await _codesRepository.GetConfirmationCode(request.Email);
        if (request.ConfirmationCode != confirmationCode)
        {
            _logger.LogInformation(
                "Someone tried to activate user account with email {Email} using invalid confirmation code.",
                request.Email);
            return Result<TokenPair>.Failure(UserErrors.InvalidVerificationCode);
        }

        var user = await _usersRepository.GetByEmail(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Someone tried to activate non-existing user account with email {Email}.", request.Email);
            return Result<TokenPair>.Failure(UserErrors.NotFoundWithEmail);
        }
        
        user.Activate();
        _usersRepository.Update(user);

        var addRefreshTokenResult = user.AddRefreshToken();
        if (addRefreshTokenResult.IsFailure)
        {
            _logger.LogError(
                "User {UserId} tried to activate their account by confirmation code" +
                " but domain error occurred on creating refresh token:\n{DomainErrorDescription}",
                user.Id, addRefreshTokenResult.Error.Description);
            return Result<TokenPair>.Failure(addRefreshTokenResult.Error);
        }

        var accessToken = _jwtProvider.GenerateUserToken(user);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("User {UserId} successfully activated their account with confirmation code.", user.Id);
        
        return Result<TokenPair>.Success(new TokenPair(accessToken, addRefreshTokenResult.Value.Token));
    }
}