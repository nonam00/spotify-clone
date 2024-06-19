using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MediatR;

using Application.Interfaces;

namespace Application.Playlists.Queries.GetPlaylistById
{
    public class GetPlaylistByIdQueryHandler : IRequestHandler<GetPlaylistByIdQuery, PlaylistVm>
    {
        private readonly ISongsDbContext _dbContext;
        private readonly IMapper _mapper;
        
        public GetPlaylistByIdQueryHandler(
            ISongsDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PlaylistVm> Handle(GetPlaylistByIdQuery request,
            CancellationToken cancellationToken)
        {
            var playlistVm = await _dbContext.Playlists
              .ProjectTo<PlaylistVm>(_mapper.ConfigurationProvider)
              .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            
            if (playlistVm is null)
            {
                throw new Exception("Playlist this such ID doesn't exist");
            }

            return playlistVm;
        }
    }
}
