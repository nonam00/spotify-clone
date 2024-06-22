using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MediatR;

using Application.Interfaces;

namespace Application.Playlists.Queries.GetPlaylistById
{
    public class GetPlaylistByIdQueryHandler(ISongsDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetPlaylistByIdQuery, PlaylistVm>
    {
        private readonly ISongsDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<PlaylistVm> Handle(GetPlaylistByIdQuery request,
            CancellationToken cancellationToken)
        {
            var playlistVm = await _dbContext.Playlists
                .AsNoTracking()
                .ProjectTo<PlaylistVm>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                ?? throw new Exception("Playlist this such ID doesn't exist");

            return playlistVm;
        }
    }
}
