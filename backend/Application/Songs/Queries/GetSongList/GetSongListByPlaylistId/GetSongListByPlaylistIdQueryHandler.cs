using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Songs.Queries.GetSongList.GetSongListByPlaylistId
{
    public class GetSongListByPlaylistIdQueryHandler
      : IRequestHandler<GetSongListByPlaylistIdQuery, SongListVm>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetSongListByPlaylistIdQueryHandler(
            ISongsDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<SongListVm> Handle(
            GetSongListByPlaylistIdQuery request,
            CancellationToken cancellationToken)
        {
            var songs = await _dbContext.PlaylistSongs
              .Where(ps => ps.PlaylistId == request.PlaylistId)
              .OrderByDescending(ps => ps.CreatedAt)
              .Select(ps => ps.Song)
              .ProjectTo<SongVm>(_mapper.ConfigurationProvider)
              .ToListAsync(cancellationToken);
            
            return new SongListVm { Songs = songs };
        }
    }
}