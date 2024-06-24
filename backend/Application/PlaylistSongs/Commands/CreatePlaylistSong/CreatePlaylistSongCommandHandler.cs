using MediatR;

using Domain;
using Application.Interfaces;

namespace Application.PlaylistSongs.Commands.CreatePlaylistSong
{
    public class CreatePlaylistSongCommandHandler(ISongsDbContext dbContext)
        : IRequestHandler<CreatePlaylistSongCommand, string>
    {
        private readonly ISongsDbContext _dbContext = dbContext;

        public async Task<string> Handle(
            CreatePlaylistSongCommand request,
            CancellationToken cancellationToken)
        {
            var ps = new PlaylistSong
            {
                PlaylistId = request.PlaylistId,
                SongId = request.SongId
            };

            await _dbContext.PlaylistSongs.AddAsync(ps, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ps.PlaylistId.ToString() + ps.SongId.ToString();
        }
    }
}
