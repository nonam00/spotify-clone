using Application.Shared.Messaging;

namespace Application.Users.Commands.DeleteRefreshToken;

public class DeleteRefreshTokenCommand : ICommand
{
    public string RefreshToken { get; init; } = null!;
}