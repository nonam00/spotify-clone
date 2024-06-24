using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListBySearchStringAndPlaylistId
{
    public class GetLikedSongListBySearchStringAndPlaylistIdQueryHandler
        (ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetLikedSongListBySearchStringAndPlaylistIdQuery, LikedSongListVm>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<LikedSongListVm> Handle(
            GetLikedSongListBySearchStringAndPlaylistIdQuery request,
            CancellationToken cancellationToken)
        {
            var liked = await _dbContext.LikedSongs
                .AsNoTracking()
                .Where(l => l.UserId == request.UserId)
                .Where(l => !_dbContext.PlaylistSongs
                    .Where(ps => ps.PlaylistId == request.PlaylistId &&
                                 ps.SongId == l.SongId)
                    .Any())
                .Where(l => EF.Functions.TrigramsSimilarity(l.Song.Title, request.SearchString) > 0.1 ||
                            EF.Functions.TrigramsSimilarity(l.Song.Author, request.SearchString) > 0.1)
                .OrderBy(l => EF.Functions.TrigramsSimilarityDistance(l.Song.Title, request.SearchString))
                    .ThenBy(l => EF.Functions.TrigramsSimilarityDistance(l.Song.Author, request.SearchString))
                .Take(20)
                .ProjectTo<LikedSongVm>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        
            return new LikedSongListVm { LikedSongs = liked };
 
        }
    }
}
