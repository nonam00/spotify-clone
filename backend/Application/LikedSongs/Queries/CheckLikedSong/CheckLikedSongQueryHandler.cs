using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.LikedSongs.Queries.CheckLikedSong
{
    public class CheckLikedSongQueryHandler(ISongsDbContext dbContext)
        : IRequestHandler<CheckLikedSongQuery, bool>
    {
        private readonly ISongsDbContext _dbContext = dbContext;

        public async Task<bool> Handle(CheckLikedSongQuery request,
            CancellationToken cancellationToken)
        {
            return await _dbContext.LikedSongs
                .AsNoTracking()  
                .Where(l => l.SongId == request.SongId &&
                            l.UserId == request.UserId)
                .AnyAsync(cancellationToken);
        }
    }
}
