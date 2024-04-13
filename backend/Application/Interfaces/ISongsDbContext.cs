using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface ISongsDbContext
    {
        DbSet<Song> Songs { get; set; }
        DbSet<LikedSong> LikedSongs { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
