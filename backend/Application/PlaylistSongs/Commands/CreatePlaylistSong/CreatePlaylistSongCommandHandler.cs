using MediatR;
using Microsoft.EntityFrameworkCore;

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
            await using var transaction = await _dbContext.BeginTransactionAsync(cancellationToken);

            try
            {
                var playlistExist = await _dbContext.Playlists
                    .AsNoTracking()  
                    .AnyAsync(p => p.UserId == request.UserId && p.Id == request.PlaylistId, cancellationToken);

                if (!playlistExist)
                {
                    throw new Exception("Playlist with such ID doesn't exist or doesn't belong to the current user");
                }
                
                var songExists = await _dbContext.Songs
                    .AsNoTracking()
                    .AnyAsync(s => s.Id == request.SongId, cancellationToken);

                if (!songExists)
                {
                    throw new Exception("Song with such ID doesn't exist");
                }
                
                var ps = new PlaylistSong
                {
                    PlaylistId = request.PlaylistId,
                    SongId = request.SongId,
                };

                await _dbContext.PlaylistSongs.AddAsync(ps, cancellationToken);

                var updatedRows = await _dbContext.Playlists
                    .Where(p => p.Id == request.PlaylistId)
                    .ExecuteUpdateAsync(p => p.SetProperty(u => u.CreatedAt, DateTime.UtcNow),
                        cancellationToken);

                if (updatedRows != 1)
                {
                    throw new Exception("Playlist with such ID doesn't exist");
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return $"{ps.PlaylistId}:{ps.SongId}";
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
