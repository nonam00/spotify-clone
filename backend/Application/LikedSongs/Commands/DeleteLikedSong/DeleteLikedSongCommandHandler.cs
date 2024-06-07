using Application.Interfaces;
using Domain;
using MediatR;

namespace Application.LikedSongs.Commands.DeleteLikedSong
{
    public class DeleteLikedSongCommandHandler : IRequestHandler<DeleteLikedSongCommand>
    {
        private readonly ISongsDbContext _dbContext;

        public DeleteLikedSongCommandHandler(ISongsDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task Handle(DeleteLikedSongCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.LikedSongs
                .FindAsync([request.UserId, request.SongId], cancellationToken);

            if(entity is null)
            {
                throw new Exception(nameof(LikedSong));
            }

            _dbContext.LikedSongs.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
