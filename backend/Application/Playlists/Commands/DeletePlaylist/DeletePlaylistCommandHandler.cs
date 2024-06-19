using MediatR;
using Microsoft.EntityFrameworkCore;

using Application.Interfaces;

namespace Application.Playlists.Commands.DeletePlaylist
{
    public class DeletePlaylistCommandHandler(ISongsDbContext dbContext)
      : IRequestHandler<DeletePlaylistCommand>
    {
        private readonly ISongsDbContext _dbContext = dbContext;

        public async Task Handle(DeletePlaylistCommand request,
            CancellationToken cancellationToken)
        {
            var playlist = await _dbContext.Playlists
              .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (playlist is null)
            {
                throw new Exception("Playlist with such ID doesn't exists");
            }

            _dbContext.Playlists.Remove(playlist);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
