using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
                .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
                ?? throw new Exception("Nonvalid current user id");

            return user;
        }
    }
}
