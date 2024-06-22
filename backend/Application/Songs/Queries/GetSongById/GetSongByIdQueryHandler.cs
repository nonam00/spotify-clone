using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Songs.Queries.GetSongById
{
    public class GetSongByIdQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetSongByIdQuery, SongVm>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<SongVm> Handle(GetSongByIdQuery request, CancellationToken cancellationToken)
        {
            var song = await _dbContext.Songs
                .AsNoTracking()
                .ProjectTo<SongVm>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(s => s.Id == request.SongId, cancellationToken)
                ?? throw new Exception("Song not found");

            return song;
        }
    }
}
