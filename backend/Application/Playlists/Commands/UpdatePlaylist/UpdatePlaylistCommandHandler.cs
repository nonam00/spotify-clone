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
            await _dbContext.Playlists
                .Where(p => p.Id == request.Id)
                .ExecuteUpdateAsync(p => p
                    .SetProperty(u => u.Title, u => request.Title ?? u.Title)
                    .SetProperty(u => u.Description, u => request.Description ?? u.Description)
                    .SetProperty(u => u.ImagePath, u => request.ImagePath ?? u.ImagePath)
                    .SetProperty(u => u.CreatedAt, DateTime.UtcNow),
                    cancellationToken);
                    
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
