using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetUserInfo
{
    public class GetUserInfoQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetUserInfoQuery, UserInfo>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<UserInfo> Handle(GetUserInfoQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
                ?? throw new Exception("Nonvalid current user id");

            var userVm = _mapper.Map<UserInfo>(user);

            return userVm;
        }
    }
}
