using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Songs.Queries.GetSongById
{
    public class GetSongByIdQueryHandler : IRequestHandler<GetSongByIdQuery, SongVm>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetSongByIdQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<SongVm> Handle(GetSongByIdQuery request, CancellationToken cancellationToken)
        {
            var song = await _dbContext.Songs
                .ProjectTo<SongVm>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(s => s.Id == request.SongId, cancellationToken);

            if (song == null)
            {
                throw new Exception("Song not found");
            }

            return song;
        }
    }
}
