using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.PlaylistSongs.Queries.CheckPlaylistSong
{
    public class CheckPlaylistSongQueryHandler(ISongsDbContext dbContext)
        : IRequestHandler<CheckPlaylistSongQuery, bool>
    {
        private readonly ISongsDbContext _dbContext = dbContext;

        public async Task<bool> Handle(CheckPlaylistSongQuery request,
            CancellationToken cancellationToken)
        {
            var _ps = await _dbContext.PlaylistSongs
                .Where(ps => ps.PlaylistId == request.PlaylistId &&
                             ps.SongId == request.SongId)
                .FirstOrDefaultAsync(cancellationToken);

            return (_ps is not null);
        }
    }
}
