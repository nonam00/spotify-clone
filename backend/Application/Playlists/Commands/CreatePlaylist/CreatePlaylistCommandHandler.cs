using MediatR;
using Microsoft.EntityFrameworkCore;

using Domain;
using Application.Interfaces;

namespace Application.Playlists.Commands.CreatePlaylist
{
    public class CreatePlaylistCommandHandler(ISongsDbContext dbContext)
        : IRequestHandler<CreatePlaylistCommand, Guid>
    {
        private readonly ISongsDbContext _dbContext = dbContext;

        public async Task<Guid> Handle(
            CreatePlaylistCommand request,
            CancellationToken cancellationToken)
        {
            var count = await _dbContext.Playlists
                .AsNoTracking()
                .Where(p => p.UserId == request.UserId)
                .CountAsync(cancellationToken);

            var playlist = new Playlist
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Title = $"Playlist #{count + 1}"
            };
            
            await _dbContext.Playlists.AddAsync(playlist, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return playlist.Id;
        }
    }
}
