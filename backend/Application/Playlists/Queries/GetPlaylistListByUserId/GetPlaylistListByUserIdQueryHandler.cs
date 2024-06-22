using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Playlists.Queries.GetPlaylistListByUserId
{
    public class GetPlaylistListByUserIdQueryHandler(
        ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetPlaylistListByUserIdQuery, PlaylistListVm>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<PlaylistListVm> Handle(
            GetPlaylistListByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            var list = await _dbContext.Playlists
                .AsNoTracking()
                .Where(p => p.UserId == request.UserId)
                .Take(10)
                .OrderByDescending(p => p.CreatedAt)
                .ProjectTo<PlaylistVm>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new PlaylistListVm { Playlists = list };
        }
    }
}

