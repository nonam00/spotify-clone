using MediatR;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<string?>
{
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public string? AvatarPath { get; set; }
}