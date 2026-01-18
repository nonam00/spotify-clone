using Microsoft.Extensions.Logging;

using Application.Moderators.Errors;
using Application.Moderators.Interfaces;
using Application.Shared.Data;
using Application.Shared.Interfaces;
using Application.Shared.Messaging;

namespace Application.Moderators.Queries.LoginByCredentials;

public class LoginByCredentialsCommandHandler : IQueryHandler<LoginByCredentialsQuery, Result<string>>
{
    private readonly IModeratorsRepository _moderatorsRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly ILogger<LoginByCredentialsCommandHandler> _logger;
    
    public LoginByCredentialsCommandHandler(
        IModeratorsRepository moderatorsRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        ILogger<LoginByCredentialsCommandHandler> logger)
    {
        _moderatorsRepository = moderatorsRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(LoginByCredentialsQuery request, CancellationToken cancellationToken)
    {
        var moderator = await _moderatorsRepository.GetByEmail(request.Email, cancellationToken);

        if (moderator == null)
        {
            return Result<string>.Failure(ModeratorErrors.InvalidCredentials);
        }

        if (!moderator.IsActive)
        {
            _logger.LogInformation("Non active moderator {moderatorId} with tried to login.", moderator.Id);
            return Result<string>.Failure(ModeratorErrors.AlreadyExistButNotActive);
        }
        
        if (!_passwordHasher.Verify(request.Password, moderator.PasswordHash))
        {
            _logger.LogInformation("Invalid login attempt by moderator {moderatorId}.", moderator.Id);
            return Result<string>.Failure(ModeratorErrors.InvalidCredentials);
        }

        var accessToken = _jwtProvider.GenerateModeratorToken(moderator);
        
        _logger.LogInformation("Generated access token for moderator {moderatorId}", moderator.Id);

        return Result<string>.Success(accessToken);
    }
}