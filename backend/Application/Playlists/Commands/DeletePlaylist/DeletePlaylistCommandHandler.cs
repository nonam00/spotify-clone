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
            var deletedRows = await _dbContext.Playlists
                .Where(p => p.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            if (deletedRows != 1)
            {
                throw new Exception("Playlist with such ID doesn't exists");
            }
        }
    }
}
