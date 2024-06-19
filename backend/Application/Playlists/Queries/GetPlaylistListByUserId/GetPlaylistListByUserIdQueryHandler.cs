using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Playlists.Queries.GetPlaylistListByUserId
{
    public class GetPlaylistListByUserIdQueryHandler
      : IRequestHandler<GetPlaylistListByUserIdQuery, PlaylistListVm>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetPlaylistListByUserIdQueryHandler(
            ISongsDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PlaylistListVm> Handle(
            GetPlaylistListByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            var list = await _dbContext.Playlists
              .Where(p => p.UserId == request.UserId)
              .ProjectTo<PlaylistVm>(_mapper.ConfigurationProvider)
              .Take(10)
              .ToListAsync(cancellationToken);

            return new PlaylistListVm { Playlists = list };
        }
    }
}

