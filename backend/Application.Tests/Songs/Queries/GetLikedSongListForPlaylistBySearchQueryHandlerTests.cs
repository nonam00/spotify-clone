using FluentAssertions;

using Domain.Models;
using Domain.ValueObjects;
using Application.Songs.Queries.GetLikedSongListForPlaylistBySearch;

namespace Application.Tests.Songs.Queries;

public class GetLikedSongListForPlaylistBySearchQueryHandlerTests : TestBaseWithPostgresFixture
{
    public GetLikedSongListForPlaylistBySearchQueryHandlerTests(PostgreSqlFixture fixture) : base(fixture) { }

    // Simple tests
    [Fact]
    public async Task Handle_ShouldReturnLikedSongsMatchingSearch_NotInPlaylist()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var playlist = user.CreatePlaylist().Value;

        var inPlaylist = user.UploadSong(
            "InPlaylistSong",
            "AuthorA",
            new FilePath("s1.mp3"),
            new FilePath("i1.jpg")).Value;
        
        var likedNotInPlaylist = user.UploadSong(
            "LikedUniqueTitle",
            "AuthorB",
            new FilePath("s2.mp3"),
            new FilePath("i2.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(inPlaylist);
        moderator.PublishSong(likedNotInPlaylist);

        user.LikeSong(inPlaylist);
        user.LikeSong(likedNotInPlaylist);
        
        playlist.AddSong(inPlaylist);

        await Context.Users.AddAsync(user);
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: user.Id, PlaylistId: playlist.Id, SearchString: "LikedUnique");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().NotBeNull();
        result.Value.Songs.Should().ContainSingle(s => s.Title == "LikedUniqueTitle");
        result.Value.Songs.Should().NotContain(s => s.Title == "InPlaylistSong");
    }

    [Fact]
    public async Task Handle_ShouldExcludeSongsAlreadyInPlaylist()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var playlist = user.CreatePlaylist().Value;
        
        var song = user.UploadSong(
            "OnlyInPlaylist",
            "Artist",
            new FilePath("s.mp3"),
            new FilePath("i.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        user.LikeSong(song);
        
        playlist.AddSong(song);

        await Context.Users.AddAsync(user);
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: user.Id, PlaylistId: playlist.Id, SearchString: "OnlyInPlaylist");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoLikedSongsMatch()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var playlist = user.CreatePlaylist().Value;
        
        var song = user.UploadSong(
            "OtherTitle",
            "OtherAuthor",
            new FilePath("s.mp3"),
            new FilePath("i.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);
        
        user.LikeSong(song);
        
        playlist.AddSong(song);

        await Context.Users.AddAsync(user);
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: user.Id, PlaylistId: playlist.Id, SearchString: "NonExistentXyZ");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyLikedSongs_ForRequestedUser()
    {
        // Arrange: two users, each with liked songs; search as first user
        var user1 = User.Create(new Email("u1@email.com"), new PasswordHash("password_hash"));
        user1.Activate();
        var user2 = User.Create(new Email("u2@email.com"), new PasswordHash("password_hash"));
        user2.Activate();

        var playlist1 = user1.CreatePlaylist().Value;

        var song1 = user1.UploadSong(
            "SharedTitle",
            "Author1",
            new FilePath("s1.mp3"),
            new FilePath("i1.jpg")).Value;
        var song2 = user2.UploadSong(
            "SharedTitle",
            "Author2",
            new FilePath("s2.mp3"),
            new FilePath("i2.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSongs([song1, song2]);

        user1.LikeSong(song1);
        user2.LikeSong(song2);

        await Context.Users.AddRangeAsync(user1, user2);
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: user1.Id, PlaylistId: playlist1.Id, SearchString: "SharedTitle");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Id == song1.Id);
        result.Value.Songs.Should().NotContain(s => s.Id == song2.Id);
    }

    [Fact]
    public async Task Handle_ShouldBeCaseInsensitive()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;

        var song = user.UploadSong(
            "MixedCaseSong",
            "Artist",
            new FilePath("s.mp3"),
            new FilePath("i.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);
        
        user.LikeSong(song);

        await Context.Users.AddAsync(user);
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: user.Id, PlaylistId: playlist.Id, SearchString: "MIXEDCASE");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Title == "MixedCaseSong");
    }

    [Fact]
    public async Task Handle_ShouldReturnMultipleLikedSongsMatchingSearch_NotInPlaylist()
    {
        // Arrange
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;

        var song1 = user.UploadSong(
            "RockSongOne"
            , "Band",
            new FilePath("s1.mp3"),
            new FilePath("i1.jpg")).Value;
        var song2 = user.UploadSong(
            "RockSongTwo",
            "Band",
            new FilePath("s2.mp3"),
            new FilePath("i2.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSongs([song1, song2]);

        user.LikeSong(song1);
        user.LikeSong(song2);

        await Context.Users.AddAsync(user);
        await Context.Moderators.AddAsync(moderator);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: user.Id, PlaylistId: playlist.Id, SearchString: "Rock");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().HaveCount(2);
        result.Value.Songs.Should().Contain(s => s.Title == "RockSongOne");
        result.Value.Songs.Should().Contain(s => s.Title == "RockSongTwo");
    }
    
    // Similarity threshold (trigram) search tests
    [Fact]
    public async Task Handle_ShouldReturnSongs_WhenSearchMatchesByTrigramSimilarity_TypoInTitle()
    {
        // Arrange: similarity threshold is 0.1 for length <= 6, 0.2 for length > 6
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var playlist = user.CreatePlaylist().Value;
        
        var song = user.UploadSong(
            "Beautiful",
            "Artist",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);
        
        user.LikeSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            user.Id, playlist.Id, "Beautifull"); // typo: extra 'l'

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Title == "Beautiful");
    }

    [Fact]
    public async Task Handle_ShouldReturnSongs_WhenSearchMatchesByTrigramSimilarity_ShortQueryTitle()
    {
        // Arrange: "Sng" vs "Song" - trigram similarity should exceed 0.1
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;

        var song = user.UploadSong("Song", "Author", new FilePath("song.mp3"), new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);
        
        user.LikeSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            user.Id, playlist.Id, "Sng");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Title == "Song");
    }

    [Fact]
    public async Task Handle_ShouldReturnSongs_WhenSearchMatchesByTrigramSimilarity_ByAuthor()
    {
        // Arrange: typo in author name
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;

        var song = user.UploadSong(
            "Track",
            "Christopher",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);
        
        user.LikeSong(song);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            user.Id, playlist.Id, "Christofer"); // typo: 'e' instead of 'h'

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Author == "Christopher");
    }

    [Fact]
    public async Task Handle_ShouldReturnSongs_WhenSearchMatchesByTrigramSimilarity_ByAny()
    {
        // Arrange: typo matches either title or author
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var playlist = user.CreatePlaylist().Value;
        
        var song = user.UploadSong(
            "Melody",
            "Beethoven",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        user.LikeSong(song);
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            user.Id, playlist.Id, "Beethven"); // typo: 'v' instead of 'o'

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Author == "Beethoven");
    }

    [Fact]
    public async Task Handle_ShouldOrderExactMatchBeforeSimilarityMatch_WhenMultipleSongsMatch()
    {
        // Arrange: one exact substring match, one trigram-only match - exact should appear first
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;

        var exactMatch = user.UploadSong(
            "RockMusic",
            "Band",
            new FilePath("s1.mp3"),
            new FilePath("i1.jpg")).Value;
        
        var similarityMatch = user.UploadSong(
            "Rock",
            "Other",
            new FilePath("s2.mp3"),
            new FilePath("i2.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSongs([exactMatch, similarityMatch]);

        user.LikeSong(exactMatch);
        user.LikeSong(similarityMatch);
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            user.Id, playlist.Id, "RockMusic");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().HaveCount(2);
        result.Value.Songs!.First().Title.Should().Be("RockMusic");
    }

    [Fact]
    public async Task Handle_ShouldReturnSongs_WhenSearchMatchesByTrigramSimilarity_ThresholdStricterForLongQuery()
    {
        // Arrange: long query (length > 6) uses threshold 0.2 - close typo should still match
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();

        var playlist = user.CreatePlaylist().Value;
        
        var song = user.UploadSong(
            "Something",
            "Someone",
            new FilePath("song.mp3"),
            new FilePath("img.jpg")).Value;
        
        var moderator = Moderator.Create(new Email("mod@e.com"), new PasswordHash("hashed_password"), "Mod");
        moderator.PublishSong(song);

        user.LikeSong(song);
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            user.Id, playlist.Id, "Somethng"); // missing 'i'

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().ContainSingle(s => s.Title == "Something");
    }
    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenUserHasNoLikedSongs()
    {
        var user = User.Create(new Email("user@email.com"), new PasswordHash("password_hash"));
        user.Activate();
        
        var playlist = user.CreatePlaylist().Value;

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: user.Id, PlaylistId: playlist.Id, SearchString: "anything");

        var result = await Mediator.Send(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().NotBeNull().And.BeEmpty();
    }
    
    // Validation tests
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsEmpty()
    {
        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: Guid.Empty, PlaylistId: Guid.NewGuid(), SearchString: "search");

        var result = await Mediator.Send(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("User ID");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPlaylistIdIsEmpty()
    {
        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: Guid.NewGuid(), PlaylistId: Guid.Empty, SearchString: "search");

        var result = await Mediator.Send(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Playlist ID");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSearchStringIsEmpty()
    {
        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: Guid.NewGuid(), PlaylistId: Guid.NewGuid(), SearchString: "");

        var result = await Mediator.Send(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Search string");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSearchStringIsWhitespaceOnly()
    {
        // Arrange
        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: Guid.NewGuid(), PlaylistId:Guid.NewGuid(),"     ");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("Search string is required");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSearchStringIsTooShort()
    {
        // Arrange
        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: Guid.NewGuid(), PlaylistId:Guid.NewGuid(),"ab");
        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("at least 3 characters");
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenSearchStringIsExactlyThreeCharacters()
    {
        // Arrange
        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: Guid.NewGuid(), PlaylistId:Guid.NewGuid(),"abc");

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Songs.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSearchStringExceedsMaxLength()
    {
        // Arrange
        var longSearch = new string('a', 101);

        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: Guid.NewGuid(), PlaylistId:Guid.NewGuid(), longSearch);
        
        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("ValidationError");
        result.Error.Description.Should().Contain("100");
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenSearchStringIsExactlyMaxLength()
    {
        // Arrange
        var maxLengthSearch = new string('a', 100); 
        
        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: Guid.NewGuid(), PlaylistId:Guid.NewGuid(),maxLengthSearch);

        // Act
        var result = await Mediator.Send(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
