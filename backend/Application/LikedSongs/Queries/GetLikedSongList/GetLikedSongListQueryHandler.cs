using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.LikedSongs.Queries.GetLikedSongList
{
    public class GetLikedSongListQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetLikedSongListQuery, LikedSongListVm>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<LikedSongListVm> Handle(
            GetLikedSongListQuery request,
            CancellationToken cancellationToken)
        {
            var songsQuery = await _dbContext.LikedSongs
                .AsNoTracking()
                .Where(liked => liked.UserId == request.UserId)
                .OrderByDescending(liked => liked.CreatedAt)
                .ProjectTo<LikedSongVm>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new LikedSongListVm { LikedSongs = songsQuery };
        }
    }
}
