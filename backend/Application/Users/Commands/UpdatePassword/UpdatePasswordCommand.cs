using MediatR;

namespace Application.Users.Commands.UpdatePassword;

public class UpdatePasswordCommand : IRequest
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}