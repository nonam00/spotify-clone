using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Shared.Models;

namespace Application.Users.Commands.ActivateUserByConfirmationCode;

public record ActivateUserByConfirmationCodeCommand(
    string Email, string ConfirmationCode) : ICommand<Result<TokenPair>>;