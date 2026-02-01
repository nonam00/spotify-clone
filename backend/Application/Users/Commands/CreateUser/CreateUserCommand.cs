using Application.Shared.Messaging;
using Domain.Common;

namespace Application.Users.Commands.CreateUser;

public record CreateUserCommand(string Email, string Password, string? FullName) : ICommand<Result>;