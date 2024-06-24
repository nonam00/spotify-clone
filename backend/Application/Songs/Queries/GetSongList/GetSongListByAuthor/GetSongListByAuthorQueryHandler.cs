using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Songs.Queries.GetSongList.GetSongListByAuthor
{
    public class GetSongListByAuthorQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetSongListByAuthorQuery, SongListVm>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<SongListVm> Handle(GetSongListByAuthorQuery request,
            CancellationToken cancellationToken)
        {
            var songsQuery = await _dbContext.Songs
                .AsNoTracking()
                .Where(song => EF.Functions.TrigramsSimilarity(song.Author, request.SearchString) > 0.1)
                .OrderBy(song => EF.Functions.TrigramsSimilarityDistance(song.Author, request.SearchString))
                .ProjectTo<SongVm>(_mapper.ConfigurationProvider)
                .Take(20)
                .ToListAsync(cancellationToken);
            return new SongListVm { Songs = songsQuery };
        }
    }
}
