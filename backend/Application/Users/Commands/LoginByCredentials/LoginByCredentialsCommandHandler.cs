using Microsoft.Extensions.Logging;

using Domain.Common;
using Application.Users.Interfaces;
using Application.Users.Errors;
using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Shared.Models;

namespace Application.Users.Commands.LoginByCredentials;

public class LoginByCredentialsCommandHandler : IQueryHandler<LoginByCredentialsCommand, Result<TokenPair>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginByCredentialsCommandHandler> _logger;
    
    public LoginByCredentialsCommandHandler(IUsersRepository usersRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork, ILogger<LoginByCredentialsCommandHandler> logger)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TokenPair>> Handle(LoginByCredentialsCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByEmailWithRefreshTokens(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogInformation("Tried to login but user with email {Email} doesnt exist.", request.Email);
            return Result<TokenPair>.Failure(UserErrors.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            _logger.LogInformation("Non-active user {UserId} tried to login.", user.Id);
            return Result<TokenPair>.Failure(UserErrors.AlreadyExistButNotActive);
        }
        
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogInformation("Invalid login attempt by user {UserId}.", user.Id);
            return Result<TokenPair>.Failure(UserErrors.InvalidCredentials);
        }

        var addRefreshTokenResult = user.AddRefreshToken();
        if (addRefreshTokenResult.IsFailure)
        {
            _logger.LogError(
                "User {UserId} tried to login" +
                " but domain error occurred on creating refresh token:\n{DomainErrorDescription}",
                user.Id, addRefreshTokenResult.Error.Description);   
            return Result<TokenPair>.Failure(addRefreshTokenResult.Error);
        }
        
        var accessToken = _jwtProvider.GenerateUserToken(user);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully generated token pair for user {userId}.", user.Id);

        return Result<TokenPair>.Success(new TokenPair(accessToken, addRefreshTokenResult.Value.Token));
    }
}