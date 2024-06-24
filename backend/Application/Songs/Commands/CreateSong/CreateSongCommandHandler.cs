using Application.Interfaces;
using Domain;
using MediatR;

namespace Application.Songs.Commands.CreateSong
{
    public class CreateSongCommandHandler(ISongsDbContext dbContext)
        : IRequestHandler<CreateSongCommand, Guid>
    {
        private readonly ISongsDbContext _dbContext = dbContext;

        public async Task<Guid> Handle(CreateSongCommand request,
            CancellationToken cancellationToken)
        {
            var song = new Song
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Title = request.Title,
                Author = request.Author,
                SongPath = request.SongPath,
                ImagePath = request.ImagePath
            };

            await _dbContext.Songs.AddAsync(song, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return song.Id;
        }
    }
}
