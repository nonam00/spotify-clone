using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Songs.Queries.GetSongList.GetSongListByAny
{
    public class GetSongListByAnyQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetSongListByAnyQuery, SongListVm>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<SongListVm> Handle(GetSongListByAnyQuery request,
            CancellationToken cancellationToken)
        {
            var songs = await _dbContext.Songs
                .AsNoTracking()
                .Where(song => EF.Functions.TrigramsSimilarity(song.Title, request.SearchString) > 0.1 ||
                               EF.Functions.TrigramsSimilarity(song.Author, request.SearchString) > 0.1)
                .OrderBy(song => EF.Functions.TrigramsSimilarityDistance(song.Title, request.SearchString))
                    .ThenBy(song => EF.Functions.TrigramsSimilarityDistance(song.Author, request.SearchString))
                    .ThenByDescending(song => song.CreatedAt)
                .ProjectTo<SongVm>(_mapper.ConfigurationProvider)
                .Take(50)
                .ToListAsync(cancellationToken);

            return new SongListVm { Songs = songs };
        }
    }
}
