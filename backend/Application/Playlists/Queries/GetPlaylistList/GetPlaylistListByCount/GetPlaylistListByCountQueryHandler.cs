using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Playlists.Queries.GetPlaylistList.GetPlaylistListByCount
{
    public class GetPlaylistListByCountQueryHandler(
        ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetPlaylistListByCountQuery, PlaylistListVm>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<PlaylistListVm> Handle(
            GetPlaylistListByCountQuery request,
            CancellationToken cancellationToken)
        {
            var playlists = await _dbContext.Playlists
                .AsNoTracking()
                .Where(p => p.UserId == request.UserId)
                .OrderByDescending(p => p.CreatedAt)
                .ProjectTo<PlaylistVm>(_mapper.ConfigurationProvider)
                .Take(request.Count)
                .ToListAsync(cancellationToken);

            return new PlaylistListVm { Playlists = playlists };
        }
    }
}
