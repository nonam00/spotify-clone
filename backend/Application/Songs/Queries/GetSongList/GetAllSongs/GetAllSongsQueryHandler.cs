using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Songs.Queries.GetSongList.GetAllSongs
{
    public class GetAllSongsQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetAllSongsQuery, SongListVm>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<SongListVm> Handle(GetAllSongsQuery request, CancellationToken cancellationToken)
        {
            var songs = await _dbContext.Songs
                .AsNoTracking()
                .ProjectTo<SongVm>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            return new SongListVm() { Songs = songs };
        }
    }
}
