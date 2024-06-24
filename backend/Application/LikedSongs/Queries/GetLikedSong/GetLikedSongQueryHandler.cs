using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.LikedSongs.Queries.GetLikedSong
{
    public class GetLikedSongQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetLikedSongQuery, LikedSongVm?>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<LikedSongVm?> Handle(GetLikedSongQuery request,
            CancellationToken cancellationToken)
        {
            var liked = await _dbContext.LikedSongs
                .AsNoTracking()  
                .Where(l => l.SongId == request.SongId &&
                            l.UserId == request.UserId)
                .ProjectTo<LikedSongVm>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return liked;
        }
    }
}
