using FluentAssertions;

using Application.Playlists.Queries.GetPlaylistById;
using Application.Playlists.Errors;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Tests.Playlists.Queries;

public class GetPlaylistByIdQueryHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ShouldReturnPlaylist_WhenPlaylistExists()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(user.Id, "My Playlist", "Description");
        
        await Context.Users.AddAsync(user);
        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();
        
        var query = new GetPlaylistByIdQuery(user.Id, playlist.Id);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be("My Playlist");
        result.Value.Description.Should().Be("Description");
        result.Value.ImagePath.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPlaylistNotFound()
    {
        // Arrange
        var query = new GetPlaylistByIdQuery(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(PlaylistErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsEmpty()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        var playlist = Playlist.Create(user.Id, "My Playlist");
        
        await Context.Users.AddAsync(user);
        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();
        
        var query = new GetPlaylistByIdQuery(Guid.Empty, playlist.Id);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("UserId");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPlaylistIdIsEmpty()
    {
        // Arrange
        var user = User.Create(
            new Email("test@example.com"),
            new PasswordHash("hashed_password"),
            "Test User");
        user.Activate();
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var query = new GetPlaylistByIdQuery(user.Id, Guid.Empty);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("PlaylistId");
    }
}
