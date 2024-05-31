using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.LikedSongs.Queries.GetLikedSongList
{
    public class GetLikedSongListQueryHandler
        : IRequestHandler<GetLikedSongListQuery, LikedSongListVm>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetLikedSongListQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<LikedSongListVm> Handle(GetLikedSongListQuery request,
            CancellationToken cancellationToken)
        {
            var songsQuery = await _dbContext.LikedSongs
                .Where(liked => liked.UserId == request.UserId)
                .OrderByDescending(liked => liked.CreatedAt)
                .ProjectTo<LikedSongVm>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new LikedSongListVm { LikedSongs = songsQuery };
        }
    }
}
