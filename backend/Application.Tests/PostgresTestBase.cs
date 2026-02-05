using Application.Shared.Messaging;
using Application.Tests.Fixtures;
using Persistence;

namespace Application.Tests;

public class PostgresTestBase : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    private readonly PostgresDbTestFixture _fixture;
    
    protected IMediator Mediator => _fixture.Mediator;
    protected AppDbContext Context => _fixture.Context;

    protected PostgresTestBase(PostgreSqlFixture postgreSqlFixture)
    {
        _fixture = new PostgresDbTestFixture(postgreSqlFixture);
    }

    public Task InitializeAsync() => _fixture.InitializeAsync();

    public Task DisposeAsync() => _fixture.DisposeAsync();
}   