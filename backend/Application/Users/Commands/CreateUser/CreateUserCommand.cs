using Application.Shared.Data;
using Application.Shared.Messaging;

namespace Application.Users.Commands.CreateUser;

public record CreateUserCommand(string Email, string Password) : ICommand<Result>;