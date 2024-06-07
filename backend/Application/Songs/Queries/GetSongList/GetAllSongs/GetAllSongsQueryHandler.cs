using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Songs.Queries.GetSongList.GetAllSongs
{
    public class GetAllSongsQueryHandler : IRequestHandler<GetAllSongsQuery, SongListVm>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetAllSongsQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<SongListVm> Handle(GetAllSongsQuery request, CancellationToken cancellationToken)
        {
            var songs = await _dbContext.Songs
                .ProjectTo<SongVm>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            return new SongListVm() { Songs = songs };
        }
    }
}
