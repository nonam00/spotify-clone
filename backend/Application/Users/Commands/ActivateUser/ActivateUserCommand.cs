using Application.Shared.Messaging;

namespace Application.Users.Commands.ActivateUser;

public class ActivateUserCommand : ICommand
{
    public string Email { get; set; } = null!;
    public string ConfirmationCode { get; set; } = null!;
}