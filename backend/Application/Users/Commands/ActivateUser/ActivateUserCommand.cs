using MediatR;

namespace Application.Users.Commands.ActivateUser;

public class ActivateUserCommand : IRequest
{
    public string Email { get; set; } = null!;
    public string ConfirmationCode { get; set; } = null!;
}