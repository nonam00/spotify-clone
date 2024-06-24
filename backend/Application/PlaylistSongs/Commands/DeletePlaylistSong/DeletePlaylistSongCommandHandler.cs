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
            var deletedRows = await _dbContext.PlaylistSongs
                .Where(ps => ps.PlaylistId == request.PlaylistId &&
                             ps.SongId == request.SongId)
                .ExecuteDeleteAsync(cancellationToken);
            
            if (deletedRows != 1) 
            {
                throw new Exception("Playlist song with such key doesn't exists");
            }

            await _dbContext.Playlists
                .Where(p => p.Id == request.PlaylistId)
                .ExecuteUpdateAsync(p => p.SetProperty(u => u.CreatedAt, DateTime.UtcNow));
        }
    }
}
