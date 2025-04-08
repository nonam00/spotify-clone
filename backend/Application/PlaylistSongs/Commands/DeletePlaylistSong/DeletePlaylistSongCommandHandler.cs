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
            await using var transaction = await _dbContext.BeginTransactionAsync(cancellationToken);
            
            try
            {
                var deletedRows = await _dbContext.PlaylistSongs
                    .Where(ps => ps.Playlist.UserId == request.UserId && 
                                 ps.PlaylistId == request.PlaylistId &&
                                 ps.SongId == request.SongId)
                    .ExecuteDeleteAsync(cancellationToken);
                
                if (deletedRows != 1) 
                {
                    throw new Exception("Relation between playlist and song with such key doesn't exist");
                }

                var updatedRows = await _dbContext.Playlists
                    .Where(p => p.UserId == request.UserId && p.Id == request.PlaylistId)
                    .ExecuteUpdateAsync(p =>
                            p.SetProperty(u => u.CreatedAt, DateTime.UtcNow), cancellationToken);

                if (updatedRows != 1)
                {
                    throw new Exception("Playlist with such ID doesn't exist");
                }
                
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
