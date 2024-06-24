using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Common.Exceptions;
using Application.Interfaces.Auth;
using Application.Interfaces;

namespace Application.Users.Queries.Login
{
    public class LoginQueryHandler(
        ISongsDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
        : IRequestHandler<LoginQuery, string>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtProvider _jwtProvider = jwtProvider;

        public async Task<string> Handle(LoginQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken)
                ?? throw new LoginException("Wrong email or password");

            var result = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if(!result)
            {
                throw new LoginException("Wrong email or password");
            }

            var token = _jwtProvider.GenerateToken(user);

            return token;
        }
    }
}
