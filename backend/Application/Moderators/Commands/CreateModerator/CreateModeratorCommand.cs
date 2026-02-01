using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Moderators.Commands.CreateModerator;

public record CreateModeratorCommand(string Email, string FullName, string Password, bool IsSuper) : ICommand<Result>;