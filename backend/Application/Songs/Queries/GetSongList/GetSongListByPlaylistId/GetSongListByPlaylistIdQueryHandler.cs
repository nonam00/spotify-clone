using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Songs.Queries.GetSongList.GetSongListByPlaylistId
{
    public class GetSongListByPlaylistIdQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetSongListByPlaylistIdQuery, SongListVm>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<SongListVm> Handle(
            GetSongListByPlaylistIdQuery request,
            CancellationToken cancellationToken)
        {
            var songs = await _dbContext.PlaylistSongs
              .AsNoTracking()
              .Where(ps => ps.PlaylistId == request.PlaylistId)
              .OrderByDescending(ps => ps.CreatedAt)
              .Select(ps => ps.Song)
              .ProjectTo<SongVm>(_mapper.ConfigurationProvider)
              .ToListAsync(cancellationToken);
            
            return new SongListVm { Songs = songs };
        }
    }
}
