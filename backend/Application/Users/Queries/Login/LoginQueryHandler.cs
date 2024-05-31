using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using Domain;
using Application.Interfaces.Auth;
using Application.Interfaces;

namespace Application.Users.Queries.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, UserVm>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public LoginQueryHandler(
            ISongsDbContext dbContext,
            IMapper mapper,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<UserVm> Handle(LoginQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

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

            var userVm = new UserVm()
            {
                AccessToken = token,
                UserInfo = _mapper.Map<User, UserInfo>(user)
            };

            return userVm;
        }
    }
}
