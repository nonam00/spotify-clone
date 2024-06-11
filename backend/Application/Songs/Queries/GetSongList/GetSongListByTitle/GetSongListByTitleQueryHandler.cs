using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Songs.Queries.GetSongList.GetSongListByTitle
{
    public class GetSongListByTitleQueryHandler : IRequestHandler<GetSongListByTitleQuery, SongListVm>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetSongListByTitleQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<SongListVm> Handle(GetSongListByTitleQuery request,
            CancellationToken cancellationToken)
        {
            var songsQuery = await _dbContext.Songs
                .Where(song => EF.Functions.TrigramsSimilarity(song.Title, request.SearchString) > 0.1)
                .OrderBy(song => EF.Functions.TrigramsSimilarityDistance(song.Title, request.SearchString))
                .ProjectTo<SongVm>(_mapper.ConfigurationProvider)
                .Take(20)
                .ToListAsync(cancellationToken);

            return new SongListVm { Songs = songsQuery };
        }
    }
}
