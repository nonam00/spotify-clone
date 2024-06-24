using Application.Interfaces;
using MediatR;

using Domain;

namespace Application.LikedSongs.Commands.CreateLikedSong
{
    public class CreateLikedSongCommandHandler(ISongsDbContext dbContext)
        : IRequestHandler<CreateLikedSongCommand, string>
    {
        private readonly ISongsDbContext _dbContext = dbContext;

        public async Task<string> Handle(CreateLikedSongCommand request,
            CancellationToken cancellationToken)
        {
            var likedSong = new LikedSong
            {
                UserId = request.UserId,
                SongId = request.SongId
            };

            await _dbContext.LikedSongs.AddAsync(likedSong, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return likedSong.UserId.ToString() + likedSong.SongId.ToString();
        }
    }
}
