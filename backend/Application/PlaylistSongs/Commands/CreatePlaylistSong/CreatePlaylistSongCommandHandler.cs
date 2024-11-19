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
            var check = await _dbContext.Playlists
                .AsNoTracking()  
                .AnyAsync(p => p.UserId == request.UserId &&
                               p.Id == request.PlaylistId,
                               cancellationToken);

            if (!check)
            {
                throw new Exception("Playlist with such ID doen't exist or doesn't belong to the current user");
            }

            var ps = new PlaylistSong
            {
                PlaylistId = request.PlaylistId,
                SongId = request.SongId,
            };

            await _dbContext.PlaylistSongs.AddAsync(ps, cancellationToken);

            int updatedRows = await _dbContext.Playlists
              .Where(p => p.Id == request.PlaylistId)
              .ExecuteUpdateAsync(p => p.SetProperty(u => u.CreatedAt, DateTime.UtcNow),
                  cancellationToken);

            if (updatedRows != 1)
            {
                throw new Exception("Playlist with such ID doesn't exist");
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ps.PlaylistId.ToString() + ps.SongId.ToString();
        }
    }
}
