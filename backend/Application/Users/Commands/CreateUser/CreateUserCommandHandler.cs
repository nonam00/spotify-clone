﻿using Domain;
using MediatR;

using Application.Interfaces;
using Application.Interfaces.Auth;

namespace Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(ISongsDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<Guid> Handle(CreateUserCommand request,
            CancellationToken cancellationToken)
        {
            var check = _dbContext.Users.FirstOrDefault(u => u.Email == request.Email);

            if (check != null)
            {
                throw new Exception("User with this email already exits");
            }

            var hashedPassword = _passwordHasher.Generate(request.Password);

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = hashedPassword
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
