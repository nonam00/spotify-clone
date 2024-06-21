using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.PlaylistSongs.Commands.DeletePlaylistSong
{
    public class DeletePlaylistSongCommandHandler(ISongsDbContext dbContext)
      : IRequestHandler<DeletePlaylistSongCommand>
    {
        private readonly ISongsDbContext _dbContext = dbContext;

        public async Task Handle(DeletePlaylistSongCommand request,
            CancellationToken cancellationToken)
        {
            var _ps = await _dbContext.PlaylistSongs
                .Where(ps => ps.PlaylistId == request.PlaylistId &&
                             ps.SongId == request.SongId)
                .FirstOrDefaultAsync(cancellationToken);

            if (_ps is null)
            {
                throw new Exception("Playlist song with such key doesn't exists");
            }

            _dbContext.PlaylistSongs.Remove(_ps);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

}
