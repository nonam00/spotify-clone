using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Shared.Models;

namespace Application.Users.Commands.ActivateUser;

public record ActivateUserCommand(string Email, string ConfirmationCode) : ICommand<Result<TokenPair>>;