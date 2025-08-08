using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Application.LikedSongs.Interfaces;
using Application.Playlists.Interfaces;
using Application.PlaylistSongs.Interfaces;
using Application.Songs.Interfaces;
using Application.Users.Interfaces;
using Persistence.Repositories;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration).AddRepositories();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // connection string from api public configuration
        var connectionString = configuration.GetConnectionString("PostgresDb")
                               ?? throw new NullReferenceException("PostgresDb connection string is null");
        // password for database from user secret or env file
        var dbPassword = configuration["DbPassword"]
                         ?? throw new NullReferenceException("DbPassword string is null");

        services.AddDbContext<SongsDbContext>(options =>
        {
            options.UseNpgsql(connectionString + dbPassword)
                .UseSnakeCaseNamingConvention();
        });
        
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ISongsRepository, SongsRepository>();
        services.AddScoped<IPlaylistsRepository, PlaylistsRepository>();
        services.AddScoped<ILikedSongsRepository, LikedSongsRepository>();
        services.AddScoped<IPlaylistsSongsRepository, PlaylistsSongsRepository>();
        
        return services;
    }
}