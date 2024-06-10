using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Songs.Queries.GetSongList.GetSongListByAuthor
{
    public class GetSongListByAuthorQueryHandler : IRequestHandler<GetSongListByAuthorQuery, SongListVm>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetSongListByAuthorQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<SongListVm> Handle(GetSongListByAuthorQuery request,
            CancellationToken cancellationToken)
        {
            var songsQuery = await _dbContext.Songs
                .Where(song => EF.Functions.Like(song.Title, $"%{request.SearchString}%"))
                .ProjectTo<SongVm>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new SongListVm { Songs = songsQuery };
        }
    }
}
