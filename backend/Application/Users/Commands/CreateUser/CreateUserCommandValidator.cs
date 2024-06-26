﻿using FluentValidation;

namespace Application.Users.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(command => command.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Nonvalid email")
                .WithErrorCode("400");

            RuleFor(command => command.Password.Length)
                .GreaterThan(8)
                .WithMessage("Password length must be greater than 8")
                .WithErrorCode("400");

            // other rules for password
        }
    }
}
