using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Application.Shared.Messaging;
using Persistence;

namespace Application.Tests.Fixtures;

/// <summary>
/// Configures in-memory db context for base test fixture 
/// </summary>
public class InMemoryDbTestFixture : BaseTestFixture, IAsyncLifetime
{
    private readonly ServiceProvider _serviceProvider;

    public IMediator Mediator { get; }
    public AppDbContext Context { get; }

    public InMemoryDbTestFixture()
    {
        ConfigureDatabase();
        
        // Getting Mediator and DbContext from DI containers with resolved dependencies
        _serviceProvider = BuildServiceProvider();
        Context = _serviceProvider.GetRequiredService<AppDbContext>();
        Mediator = _serviceProvider.GetRequiredService<IMediator>();
    }

    private void ConfigureDatabase()
    {
        Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .UseSnakeCaseNamingConvention()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .ConfigureWarnings(w =>
                    w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });
    }

    public Task InitializeAsync() => Context.Database.EnsureCreatedAsync();

    public async Task DisposeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await Context.DisposeAsync();
        await _serviceProvider.DisposeAsync();
        
        GC.SuppressFinalize(this);
    }
}