using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListByPlaylistId
{
    public class GetLikedSongListByPlaylistIdQueryHandler(
        ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetLikedSongListByPlaylistIdQuery, LikedSongListVm>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<LikedSongListVm> Handle(
            GetLikedSongListByPlaylistIdQuery request,
            CancellationToken cancellationToken)
        {
            var liked = await _dbContext.LikedSongs
                .AsNoTracking()
                .Where(l => l.UserId == request.UserId)
                .Where(l => !_dbContext.PlaylistSongs
                    .Any(ps => ps.PlaylistId == request.PlaylistId &&
                               ps.SongId == l.SongId))
                .OrderByDescending(l => l.CreatedAt)
                .ProjectTo<LikedSongVm>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        
            return new LikedSongListVm { LikedSongs = liked };
        }
    }
}
