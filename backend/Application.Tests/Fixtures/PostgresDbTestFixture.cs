using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

using Persistence;
using Application.Shared.Messaging;

namespace Application.Tests.Fixtures;

/// <summary>
/// Configures test container postgresql db context from postgres fixture for base test fixture 
/// </summary>
public class PostgresDbTestFixture : BaseTestFixture, IAsyncLifetime
{
    private readonly PostgreSqlFixture _postgreSqlFixture;
    private readonly ServiceProvider _serviceProvider;

    public IMediator Mediator { get; }
    public AppDbContext Context { get; }

    public PostgresDbTestFixture(PostgreSqlFixture postgreSqlFixture)
    {
        _postgreSqlFixture = postgreSqlFixture;
        
        ConfigureDatabase();
        
        // Getting Mediator and DbContext from DI containers with resolved dependencies
        _serviceProvider = BuildServiceProvider();
        Context = _serviceProvider.GetRequiredService<AppDbContext>();
        Mediator = _serviceProvider.GetRequiredService<IMediator>();
        
        InitializeDatabase();
    }

    private void ConfigureDatabase()
    {
        Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(_postgreSqlFixture.ConnectionString)
                .UseSnakeCaseNamingConvention()
                .EnableSensitiveDataLogging();
            options.ConfigureWarnings(c =>
                c.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
    }

    private void InitializeDatabase() => Context.Database.EnsureCreated();

    public Task InitializeAsync() => ClearDatabaseAsync();
    
    // Clearing database before tests because db creates for tests collection and not for each test
    private Task ClearDatabaseAsync()
    {
        // Clearing all tables using truncate
        return Context.Database.ExecuteSqlRawAsync(@"
            DO $$ 
            DECLARE
                r RECORD;
            BEGIN
                FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public') LOOP
                    EXECUTE 'TRUNCATE TABLE ' || quote_ident(r.tablename) || ' CASCADE';
                END LOOP;
            END $$;");
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
        await _serviceProvider.DisposeAsync();
    }
}