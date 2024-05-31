using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetUserInfo
{
    public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserInfo>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetUserInfoQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<UserInfo> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                throw new Exception("Nonvalid current user id");
            }

            return user;
        }
    }
}
