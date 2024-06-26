using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Songs.Queries.GetSongList.GetNewestSongList
{
    public class GetNewestSongListQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetNewestSongListQuery, SongListVm>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<SongListVm> Handle(GetNewestSongListQuery request,
            CancellationToken cancellationToken)
        {
            var songs = await _dbContext.Songs
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .ProjectTo<SongVm>(_mapper.ConfigurationProvider)
                .Take(100)
                .ToListAsync(cancellationToken);

            return new SongListVm { Songs = songs };
        }
    }
}
