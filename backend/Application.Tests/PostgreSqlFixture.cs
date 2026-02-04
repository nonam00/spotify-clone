using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tests;

/// <summary>
/// Fixture for test that need a real PostgreSQL because of specific methods and extensions
/// Runs container once per test class and applies migrations
/// </summary>
public class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18.0-alpine")
        .WithDatabase("spotify-clone-test-db")
        .WithUsername("postgres")
        .WithPassword("password")
        .WithCleanUp(true)
        .Build();

    /// <summary>
    /// Connection string for AppDbContext registration in DI (TestBaseWithPostgresFixture).
    /// </summary>
    public string ConnectionString { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .EnableSensitiveDataLogging()
            .Options;

        await using var dbContext = new AppDbContext(options);
        
        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE EXTENSION IF NOT EXISTS pg_trgm;
            CREATE EXTENSION IF NOT EXISTS btree_gin;
            CREATE EXTENSION IF NOT EXISTS unaccent;

            CREATE OR REPLACE FUNCTION immutable_unaccent(regdictionary, text)
                RETURNS text
                LANGUAGE c IMMUTABLE PARALLEL SAFE STRICT AS
            '$libdir/unaccent', 'unaccent_dict';

            CREATE OR REPLACE FUNCTION f_unaccent(text)
                RETURNS text
                LANGUAGE sql IMMUTABLE PARALLEL SAFE STRICT
            RETURN immutable_unaccent(regdictionary 'unaccent', $1);          
            """);
    }
    
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}