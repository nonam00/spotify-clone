using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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
        services.AddDatabases(configuration).AddRepositories();
        return services;
    }

    private static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnectionString = configuration.GetConnectionString("PostgresDb") 
             ?? throw new NullReferenceException("PostgresDb connection string is null");
        var postgresDbPassword = configuration["DbPassword"] ?? throw new NullReferenceException("DbPassword string is null");
        
        services.AddDbContext<SongsDbContext>(options =>
        {
            options.UseNpgsql(postgresConnectionString + postgresDbPassword)
                .UseSnakeCaseNamingConvention();
        });
        
        
        var redisConnectionString = configuration["RedisConnectionString"]
            ?? throw new NullReferenceException("Redis connection string is null");
                
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnectionString));
        
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ISongsRepository, SongsRepository>();
        services.AddScoped<IPlaylistsRepository, PlaylistsRepository>();
        services.AddScoped<ILikedSongsRepository, LikedSongsRepository>();
        services.AddScoped<IPlaylistsSongsRepository, PlaylistsSongsRepository>();
        services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();

        services.AddScoped<IConfirmationCodesRepository, ConfirmationCodesRepository>();
        
        return services;
    }
}