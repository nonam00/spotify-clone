using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Interfaces;

public interface ISongsDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Song> Songs { get; set; }
    DbSet<LikedSong> LikedSongs { get; set; }
    DbSet<Playlist> Playlists { get; set; }
    DbSet<PlaylistSong> PlaylistSongs { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}