using Moq;

using Application.Shared.Interfaces;
using Application.Shared.Messaging;
using Application.Tests.Fixtures;
using Persistence;

namespace Application.Tests;

public abstract class InMemoryTestBase : IAsyncLifetime
{
    private readonly InMemoryDbTestFixture _fixture = new();
    protected IMediator Mediator => _fixture.Mediator;
    protected AppDbContext Context => _fixture.Context;
    protected Mock<IPasswordHasher> PasswordHasherMock => _fixture.PasswordHasherMock;

    public Task InitializeAsync() => _fixture.InitializeAsync();

    public Task DisposeAsync() => _fixture.DisposeAsync();
}