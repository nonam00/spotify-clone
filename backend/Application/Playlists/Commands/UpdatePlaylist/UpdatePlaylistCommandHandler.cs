using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Playlists.Commands.UpdatePlaylist
{
    public class UpdatePlaylistCommandHandler(ISongsDbContext dbContext)
      : IRequestHandler<UpdatePlaylistCommand>
    {
        private readonly ISongsDbContext _dbContext = dbContext;

        public async Task Handle(UpdatePlaylistCommand request,
            CancellationToken cancellationToken)
        {
            var playlist = await _dbContext.Playlists
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (playlist is null)
            {
                throw new Exception("Playlist with such ID doesn't exists");
            }

            if (request.Title != null && request.Title.Trim() != string.Empty)
            {
                playlist.Title = request.Title.Trim();
            }

            if (request.Description != null && request.Description.Trim() != string.Empty)
            {
                playlist.Description = request.Description.Trim();
            }

            if (request.ImagePath != null && request.ImagePath != string.Empty)
            {
                playlist.ImagePath = request.ImagePath;
            }
            
            playlist.CreatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
