using MediatR;

namespace Application.Users.Commands.DeleteRefreshToken;

public class DeleteRefreshTokenQuery : IRequest
{
    public string RefreshToken { get; init; } = null!;
}