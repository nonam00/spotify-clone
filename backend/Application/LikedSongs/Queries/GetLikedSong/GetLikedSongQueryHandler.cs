using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;
using Application.Users.Queries;

namespace Application.LikedSongs.Queries.GetLikedSong
{
    public class GetLikedSongQueryHandler : IRequestHandler<GetLikedSongQuery, LikedSongVm?>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetLikedSongQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<LikedSongVm?> Handle(GetLikedSongQuery request,
            CancellationToken cancellationToken)
        {
            var liked = await _dbContext.LikedSongs
                .Where(l => l.SongId == request.SongId &&
                            l.UserId == request.UserId)
                .ProjectTo<LikedSongVm>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return liked;
        }
    }
}
