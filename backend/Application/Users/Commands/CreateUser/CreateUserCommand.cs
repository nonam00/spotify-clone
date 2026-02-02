using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Users.Commands.CreateUser;

public record CreateUserCommand(string Email, string Password, string? FullName) : ICommand<Result>;