using Application.Interfaces;
using Domain;
using MediatR;

namespace Application.LikedSongs.Commands.DeleteLikedSong
{
    public class DeleteLikedSongCommandHandler(ISongsDbContext dbContext)
        : IRequestHandler<DeleteLikedSongCommand>
    {
        private readonly ISongsDbContext _dbContext = dbContext;

        public async Task Handle(DeleteLikedSongCommand request,
            CancellationToken cancellationToken)
        {
            var liked = await _dbContext.LikedSongs
                .FindAsync([request.UserId, request.SongId], cancellationToken)
                ?? throw new Exception(nameof(LikedSong));

            _dbContext.LikedSongs.Remove(liked);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
