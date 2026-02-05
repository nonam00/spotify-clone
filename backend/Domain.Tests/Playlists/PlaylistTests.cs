using Domain.Errors;
using Domain.Events;
using Domain.Models;
using Domain.Tests.Helpers;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.Tests.Playlists;

public class PlaylistTests
{
    private readonly Guid _userId = Guid.NewGuid();
    
    private readonly FilePath _testImagePath = new("playlist.jpg");
    private readonly Song _publishedSong = SongHelpers.CreateSong(isPublished: true);
    private readonly Song _unpublishedSong = SongHelpers.CreateSong();

    [Fact]
    public void Create_ShouldCreatePlaylistWithCorrectProperties()
    {
        // Arrange
        const string title = "My Playlist";
        const string description = "My favorite songs";

        // Act
        var result = Playlist.Create(_userId, title, description, _testImagePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var playlist = result.Value;
            
        playlist.Id.Should().NotBe(Guid.Empty);
        playlist.UserId.Should().Be(_userId);
        playlist.Title.Should().Be(title);
        playlist.Description.Should().Be(description);
        playlist.ImagePath.Should().Be(_testImagePath);
        playlist.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        playlist.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        playlist.PlaylistSongs.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyTitle_ShouldReturnFailure(string? title)
    {
        // Act
        var result = Playlist.Create(_userId, title, "Description", _testImagePath);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PlaylistDomainErrors.EmptyTitle);
    }

    [Fact]
    public void Create_ShouldTrimTitleAndDescription()
    {
        // Arrange
        const string title = "  My Playlist  ";
        const string description = "  My favorite songs  ";

        // Act
        var result = Playlist.Create(_userId, title, description, _testImagePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var playlist = result.Value;
        playlist.Title.Should().Be("My Playlist");
        playlist.Description.Should().Be("My favorite songs");
    }

    [Fact]
    public void Create_WithNullImagePath_ShouldUseDefaultNullPath()
    {
        // Act
        var result = Playlist.Create(_userId, "Title", "Description", null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ImagePath.Value.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithNullDescription_ShouldSetNullDescription()
    {
        // Act
        var result = Playlist.Create(_userId, "Title", null, _testImagePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public void UpdateDetails_ShouldUpdatePropertiesSuccessfully()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        const string newTitle = "Updated Playlist";
        const string newDescription = "Updated Description";
        var newImagePath = new FilePath("/images/new.jpg");

        // Act
        var result = playlist.UpdateDetails(newTitle, newDescription, newImagePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        playlist.Title.Should().Be(newTitle);
        playlist.Description.Should().Be(newDescription);
        playlist.ImagePath.Should().Be(newImagePath);
        playlist.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateDetails_WhenImageChanged_ShouldRaiseEvent()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId, withImage: true);
        var oldImagePath = playlist.ImagePath;
        var newImagePath = new FilePath("/images/new.jpg");

        // Act
        var result = playlist.UpdateDetails("New Title", "New Description", newImagePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        playlist.DomainEvents.Should().ContainSingle(e => e is PlaylistImageChangedEvent);
        var domainEvent = playlist.DomainEvents.OfType<PlaylistImageChangedEvent>().First();
        domainEvent.PlaylistId.Should().Be(playlist.Id);
        domainEvent.OldImagePath.Should().Be(oldImagePath);
    }

    [Fact]
    public void UpdateDetails_WhenImageNotChanged_ShouldNotRaiseEvent()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId, withImage: true);
        playlist.CleanDomainEvents();

        // Act
        var result = playlist.UpdateDetails("New Title", "New Description", playlist.ImagePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        playlist.DomainEvents.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateDetails_WithEmptyTitle_ShouldReturnFailure(string? title)
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);

        // Act
        var result = playlist.UpdateDetails(title, "Description", _testImagePath);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PlaylistDomainErrors.EmptyTitle);
        playlist.UpdatedAt.Should().BeBefore(DateTime.UtcNow);
    }

    [Fact]
    public void AddSong_WithPublishedSong_ShouldAddSuccessfully()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);

        // Act
        var result = playlist.AddSong(_publishedSong);

        // Assert
        result.IsSuccess.Should().BeTrue();
        playlist.PlaylistSongs.Should().ContainSingle();
        playlist.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            
        var playlistSong = playlist.PlaylistSongs.First();
        playlistSong.PlaylistId.Should().Be(playlist.Id);
        playlistSong.SongId.Should().Be(_publishedSong.Id);
        playlistSong.Order.Should().Be(1);
    }

    [Fact]
    public void AddSong_WithUnpublishedSong_ShouldReturnFailure()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);

        // Act
        var result = playlist.AddSong(_unpublishedSong);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PlaylistDomainErrors.CannotPerformActionsWithUnpublishedSong);
        playlist.PlaylistSongs.Should().BeEmpty();
    }

    [Fact]
    public void AddSong_WhenSongAlreadyInPlaylist_ShouldReturnFailure()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        playlist.AddSong(_publishedSong);

        // Act
        var result = playlist.AddSong(_publishedSong);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PlaylistDomainErrors.AlreadyContainsSong);
        playlist.PlaylistSongs.Should().ContainSingle();
    }

    [Fact]
    public void AddSong_ShouldIncrementOrderForEachAddedSong()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        
        var song1 = SongHelpers.CreateSong();
        var song2 = SongHelpers.CreateSong();

        song1.Publish();
        song2.Publish();

        // Act
        playlist.AddSong(song1);
        playlist.AddSong(song2);

        // Assert
        playlist.PlaylistSongs.Should().HaveCount(2);
        playlist.PlaylistSongs.ElementAt(0).Order.Should().Be(1);
        playlist.PlaylistSongs.ElementAt(1).Order.Should().Be(2);
    }

    [Fact]
    public void AddSongs_WithPublishedSongs_ShouldAddAllSongs()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        var songs = SongHelpers.CreateSongList(3, isPublished: true);

        // Act
        var result = playlist.AddSongs(songs);

        // Assert
        result.IsSuccess.Should().BeTrue();
        playlist.PlaylistSongs.Should().HaveCount(3);
        playlist.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            
        for (var i = 0; i < songs.Count; i++)
        {
            var playlistSong = playlist.PlaylistSongs.ElementAt(i);
            playlistSong.SongId.Should().Be(songs[i].Id);
            playlistSong.Order.Should().Be(i + 1);
        }
    }

    [Fact]
    public void AddSongs_WhenAnySongIsUnpublished_ShouldReturnFailure()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        
        var songs = new List<Song>
        {
            SongHelpers.CreateSong(isPublished: true),
            SongHelpers.CreateSong(isPublished: false), // Unpublished
            SongHelpers.CreateSong(isPublished: true)
        };

        // Act
        var result = playlist.AddSongs(songs);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PlaylistDomainErrors.CannotPerformActionsWithUnpublishedSong);
        playlist.PlaylistSongs.Should().BeEmpty();
    }

    [Fact]
    public void AddSongs_WhenAnySongAlreadyInPlaylist_ShouldReturnFailure()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        var existingSong = SongHelpers.CreateSong(isPublished: true);
        playlist.AddSong(existingSong);
            
        var songs = new List<Song>
        {
            existingSong, // Already in playlist
            SongHelpers.CreateSong(isPublished: true)
        };

        // Act
        var result = playlist.AddSongs(songs);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PlaylistDomainErrors.AlreadyContainsSong);
        playlist.PlaylistSongs.Should().ContainSingle();
    }

    [Fact]
    public void RemoveSong_WhenSongIsInPlaylist_ShouldRemoveSuccessfully()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        playlist.AddSong(_publishedSong);
        playlist.PlaylistSongs.Should().ContainSingle();

        // Act
        var result = playlist.RemoveSong(_publishedSong.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        playlist.PlaylistSongs.Should().BeEmpty();
        playlist.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RemoveSong_WhenSongNotInPlaylist_ShouldReturnFailure()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);

        // Act
        var result = playlist.RemoveSong(Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PlaylistDomainErrors.DoesntContainSong);
    }

    [Fact]
    public void RemoveSong_ShouldMaintainOrderOfRemainingSongs()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        
        var song1 = SongHelpers.CreateSong(isPublished: true);
        var song2 = SongHelpers.CreateSong(isPublished: true);
        var song3 = SongHelpers.CreateSong(isPublished: true);
            
        playlist.AddSong(song1);
        playlist.AddSong(song2);
        playlist.AddSong(song3);

        // Act
        playlist.RemoveSong(song2.Id);

        // Assert
        playlist.PlaylistSongs.Should().HaveCount(2);
        playlist.PlaylistSongs.ElementAt(0).SongId.Should().Be(song1.Id);
        playlist.PlaylistSongs.ElementAt(0).Order.Should().Be(1);
        playlist.PlaylistSongs.ElementAt(1).SongId.Should().Be(song3.Id);
        playlist.PlaylistSongs.ElementAt(1).Order.Should().Be(3); // Order should remain 3
    }

    [Fact]
    public void ReorderSongs_ShouldReorderSongsSuccessfully()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        
        var song1 = SongHelpers.CreateSong(isPublished: true);
        var song2 = SongHelpers.CreateSong(isPublished: true);
        var song3 = SongHelpers.CreateSong(isPublished: true);
            
        playlist.AddSong(song1);
        playlist.AddSong(song2);
        playlist.AddSong(song3);

        var newOrder = new List<Guid> { song3.Id, song1.Id, song2.Id };

        // Act
        var result = playlist.ReorderSongs(newOrder);

        // Assert
        result.IsSuccess.Should().BeTrue();
            
        var reorderedSongs = playlist.PlaylistSongs
            .OrderBy(ps => ps.Order)
            .Select(ps => ps.SongId)
            .ToList();
                
        reorderedSongs.Should().Equal(newOrder);
            
        for (var i = 0; i < newOrder.Count; i++)
        {
            var playlistSong = playlist.PlaylistSongs.First(ps => ps.SongId == newOrder[i]);
            playlistSong.Order.Should().Be(i + 1);
        }
    }

    [Fact]
    public void ReorderSongs_WithMissingSong_ShouldReturnFailure()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        var song1 = SongHelpers.CreateSong(isPublished: true);
        playlist.AddSong(song1);

        var invalidOrder = new List<Guid> { Guid.NewGuid() }; // Song not in playlist

        // Act
        var result = playlist.ReorderSongs(invalidOrder);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PlaylistDomainErrors.DoesntContainSong);
    }

    [Fact]
    public void ReorderSongs_WithDifferentCount_ShouldReturnFailure()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        var song1 = SongHelpers.CreateSong(isPublished: true);
        var song2 = SongHelpers.CreateSong(isPublished: true);
            
        playlist.AddSong(song1);
        playlist.AddSong(song2);

        var invalidOrder = new List<Guid> { song1.Id }; // Missing song2

        // Act
        var result = playlist.ReorderSongs(invalidOrder);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PlaylistDomainErrors.InvalidReorderList);
    }

    [Fact]
    public void ReorderSongs_WhenOrderIsSame_ShouldStillSucceed()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        var song1 = SongHelpers.CreateSong(isPublished: true);
        var song2 = SongHelpers.CreateSong(isPublished: true);
            
        playlist.AddSong(song1);
        playlist.AddSong(song2);

        var sameOrder = new List<Guid> { song1.Id, song2.Id };

        // Act
        var result = playlist.ReorderSongs(sameOrder);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ContainsSong_ShouldReturnCorrectState()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        playlist.AddSong(_publishedSong);

        // Act & Assert
        playlist.PlaylistSongs.Any(ps => ps.SongId == _publishedSong.Id).Should().BeTrue();
        playlist.PlaylistSongs.Any(ps => ps.SongId == Guid.NewGuid()).Should().BeFalse();
    }

    [Fact]
    public void ContainsSongs_ShouldReturnCorrectState()
    {
        // Arrange
        var playlist = PlaylistHelpers.CreatePlaylist(_userId);
        
        var song1 = SongHelpers.CreateSong(isPublished: true);
        var song2 = SongHelpers.CreateSong(isPublished: true);
        var song3 = SongHelpers.CreateSong(isPublished: true);
            
        playlist.AddSong(song1);
        playlist.AddSong(song2);

        var songIds = new List<Guid> { song1.Id, song2.Id, song3.Id };

        // Act & Assert
        playlist.PlaylistSongs.Any(ps => songIds.Contains(ps.SongId)).Should().BeTrue(); // Contains at least one
        playlist.PlaylistSongs.All(ps => songIds.Contains(ps.SongId)).Should().BeTrue(); // All playlist songs are in the list
    }
}