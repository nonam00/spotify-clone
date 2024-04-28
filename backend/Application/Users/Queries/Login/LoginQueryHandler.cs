using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces.Auth;
using Application.Interfaces;

namespace Application.Users.Queries.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, string>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public LoginQueryHandler(
            ISongsDbContext dbContext,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<string> Handle(LoginQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if(user == null)
            {
                throw new Exception("User doesn't exits");
            }

            var result = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if(result == false)
            {
                throw new Exception("Wrong password");
            }

            var token = _jwtProvider.GenerateToken(user);

            return token;
        }
    }
}
