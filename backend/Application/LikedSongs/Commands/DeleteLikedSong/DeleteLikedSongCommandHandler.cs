using MediatR;
using Microsoft.EntityFrameworkCore;

using Domain;
using Application.Interfaces;

namespace Application.LikedSongs.Commands.DeleteLikedSong
{
    public class DeleteLikedSongCommandHandler(ISongsDbContext dbContext)
        : IRequestHandler<DeleteLikedSongCommand>
    {
        private readonly ISongsDbContext _dbContext = dbContext;

        public async Task Handle(DeleteLikedSongCommand request,
            CancellationToken cancellationToken)
        {
            var deletedRows = await _dbContext.LikedSongs
                .Where(l => l.UserId == request.UserId && 
                            l.SongId == request.SongId)
                .ExecuteDeleteAsync(cancellationToken);
            
            if (deletedRows != 1)
            {
                throw new Exception(nameof(LikedSong));
            }
        }
    }
}
