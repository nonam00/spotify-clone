using FluentAssertions;

using Application.Playlists.Queries.GetPlaylistListByCount;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Playlists.Queries;

public class GetPlaylistListByCountQueryHandlerTests : InMemoryTestBase
{
    [Fact]
    public async Task Handle_Should_ReturnPlaylists_WhenValid()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();

        user.CreatePlaylist();
        var playlist2 = user.CreatePlaylist().Value; 
        var playlist3 = user.CreatePlaylist().Value;

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        const int count = 2;
        
        var query = new GetPlaylistListByCountQuery(user.Id, count);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Playlists!.Count.Should().Be(count);
        result.Value.Playlists[0].Id.Should().Be(playlist3.Id);
        result.Value.Playlists[1].Id.Should().Be(playlist2.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationFailure_WhenCountIsNotValid()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();

        user.CreatePlaylist();

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        const int count = 0;

        var query = new GetPlaylistListByCountQuery(user.Id, count);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
    }
}