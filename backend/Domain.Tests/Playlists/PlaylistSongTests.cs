using Domain.Errors;
using Domain.Models;
using FluentAssertions;

namespace Domain.Tests.Playlists;

public class PlaylistSongTests
{
    [Fact]
    public void Create_ShouldCreatePlaylistSongWithCorrectProperties()
    {
        // Arrange
        var playlistId = Guid.NewGuid();
        var songId = Guid.NewGuid();
        const int order = 1;

        // Act
        var result = PlaylistSong.Create(playlistId, songId, order);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var playlistSong = result.Value;
            
        playlistSong.PlaylistId.Should().Be(playlistId);
        playlistSong.SongId.Should().Be(songId);
        playlistSong.Order.Should().Be(order);
        playlistSong.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        playlistSong.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Create_WithInvalidOrder_ShouldReturnFailure(int order)
    {
        // Act
        var result = PlaylistSong.Create(Guid.NewGuid(), Guid.NewGuid(), order);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PlaylistDomainErrors.SongOrderCannotBeLessOrEqualToZero);
    }

    [Fact]
    public void ChangeOrder_ShouldUpdateOrderSuccessfully()
    {
        // Arrange
        var playlistSong = PlaylistSong.Create(Guid.NewGuid(), Guid.NewGuid(), 1).Value;
        var originalUpdatedAt = playlistSong.UpdatedAt;
            
        // Act
        var result = playlistSong.ChangeOrder(2);

        // Assert
        result.IsSuccess.Should().BeTrue();
        playlistSong.Order.Should().Be(2);
        playlistSong.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void ChangeOrder_WithInvalidOrder_ShouldReturnFailure(int newOrder)
    {
        // Arrange
        var playlistSong = PlaylistSong.Create(Guid.NewGuid(), Guid.NewGuid(), 1).Value;

        // Act
        var result = playlistSong.ChangeOrder(newOrder);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PlaylistDomainErrors.SongOrderCannotBeLessOrEqualToZero);
        playlistSong.Order.Should().Be(1);
    }

    [Fact]
    public void ChangeOrder_WhenOrderIsSame_ShouldReturnFailure()
    {
        // Arrange
        var playlistSong = PlaylistSong.Create(Guid.NewGuid(), Guid.NewGuid(), 1).Value;

        // Act
        var result = playlistSong.ChangeOrder(1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PlaylistDomainErrors.NewSongOrderCannotBeEqualToOld);
        playlistSong.Order.Should().Be(1);
    }
}