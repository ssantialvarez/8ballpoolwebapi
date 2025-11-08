using _8BallPool.Business.DTOs;
using _8BallPool.Business.Modules;
using _8BallPool.Data.Interfaces;
using _8BallPool.Data.Models;
using FluentAssertions;
using Moq;

namespace _8BallPool.Business.Tests.Modules;

public class MatchModuleTests
{
    private readonly Mock<IMatchRepository> _mockMatchRepository;
    private readonly Mock<IPlayerRepository> _mockPlayerRepository;
    private readonly MatchModule _matchModule;

    public MatchModuleTests()
    {
        _mockMatchRepository = new Mock<IMatchRepository>();
        _mockPlayerRepository = new Mock<IPlayerRepository>();
        _matchModule = new MatchModule(_mockMatchRepository.Object, _mockPlayerRepository.Object);
    }

    [Fact]
    public async Task GetMatchByIdAsync_WhenMatchExists_ReturnsMatch()
    {
        // Arrange
        var matchId = 1;
        var startTime = DateTime.UtcNow;
        var expectedMatch = new _8BallPool.Data.Models.Match 
        { 
            Id = matchId, 
            Player1Id = 1, 
            Player2Id = 2,
            StartTime = startTime
        };
        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync(expectedMatch);

        // Act
        var result = await _matchModule.GetMatchByIdAsync(matchId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(matchId);
        result.Player1Id.Should().Be(1);
        result.Player2Id.Should().Be(2);
        result.StartTime.Should().BeCloseTo(startTime, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task AddMatchAsync_WithValidPlayers_CreatesMatch()
    {
        // Arrange
        var matchDto = new MatchDto 
        { 
            Player1Id = 1, 
            Player2Id = 2,
            StartTime = DateTime.UtcNow
        };
        var player1 = new Player { Id = 1, Name = "Player 1", Auth0_id = "auth0|1", Profile_picture_url = "url1" };
        var player2 = new Player { Id = 2, Name = "Player 2", Auth0_id = "auth0|2", Profile_picture_url = "url2" };
        var createdMatch = new _8BallPool.Data.Models.Match 
        { 
            Id = 1, 
            Player1Id = 1, 
            Player2Id = 2,
            StartTime = matchDto.StartTime
        };

        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(1)).ReturnsAsync(player1);
        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(2)).ReturnsAsync(player2);
        _mockMatchRepository.Setup(r => r.AddMatchAsync(It.IsAny<_8BallPool.Data.Models.Match>()))
            .ReturnsAsync(createdMatch);

        // Act
        var result = await _matchModule.AddMatchAsync(matchDto);

        // Assert
        result.Should().NotBeNull();
        result!.Player1Id.Should().Be(1);
        result.Player2Id.Should().Be(2);
        _mockMatchRepository.Verify(r => r.AddMatchAsync(It.IsAny<_8BallPool.Data.Models.Match>()), Times.Once);
    }

    [Fact]
    public async Task AddMatchAsync_WithInvalidPlayer_ReturnsNull()
    {
        // Arrange
        var matchDto = new MatchDto 
        { 
            Player1Id = 2, 
            Player2Id = 999,
            StartTime = DateTime.UtcNow
        };
        var player2 = new Player { Id = 2, Name = "Player 2", Auth0_id = "auth0|2", Profile_picture_url = "url1" };
        Player? player1 = null;
        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(2)).ReturnsAsync(player2);
        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(999)).ReturnsAsync(player1);

        // Act
        var result = await _matchModule.AddMatchAsync(matchDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task FinishMatchAsync_WithValidData_SetsEndTimeAndWinner()
    {
        // Arrange
        var matchId = 1;
        var winnerId = 1;
        var auth0Id = "auth0|1";
        var existingMatch = new _8BallPool.Data.Models.Match 
        { 
            Id = matchId, 
            Player1Id = 1, 
            Player2Id = 2,
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = null
        };
        var player = new Player { Id = 1, Name = "Player 1", Auth0_id = auth0Id, Profile_picture_url = "url1" };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId)).ReturnsAsync(existingMatch);
        _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id)).ReturnsAsync(player);

        // Act
        var result = await _matchModule.FinishMatchAsync(matchId, winnerId, auth0Id);

        // Assert
        result.Should().NotBeNull();
        result!.WinnerId.Should().Be(winnerId);
        result.EndTime.Should().NotBeNull();
    }

    [Fact]
    public async Task FinishMatchAsync_WhenMatchAlreadyFinished_ThrowsInvalidOperationException()
    {
        // Arrange
        var matchId = 1;
        var winnerId = 1;
        var auth0Id = "auth0|1";
        var existingMatch = new _8BallPool.Data.Models.Match 
        { 
            Id = matchId, 
            Player1Id = 1, 
            Player2Id = 2,
            StartTime = DateTime.UtcNow.AddHours(-2),
            EndTime = DateTime.UtcNow.AddHours(-1),
            WinnerId = 1
        };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId)).ReturnsAsync(existingMatch);

        // Act & Assert
        await FluentActions.Invoking(async () => await _matchModule.FinishMatchAsync(matchId, winnerId, auth0Id))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Match is already finished.");
    }

    [Fact]
    public async Task GetMatchesByPlayerIdAsync_WithValidPlayerId_ReturnsMatches()
    {
        // Arrange
        var playerId = 1;
        var player = new Player { Id = playerId, Name = "Player 1", Auth0_id = "auth0|1", Profile_picture_url = "url1" };
        var matches = new List<_8BallPool.Data.Models.Match>
        {
            new _8BallPool.Data.Models.Match { Id = 1, Player1Id = playerId, Player2Id = 2, StartTime = DateTime.UtcNow },
            new _8BallPool.Data.Models.Match { Id = 2, Player1Id = 3, Player2Id = playerId, StartTime = DateTime.UtcNow }
        };

        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId)).ReturnsAsync(player);
        _mockMatchRepository.Setup(r => r.GetMatchesByPlayerIdAsync(playerId)).ReturnsAsync(matches);

        // Act
        var result = await _matchModule.GetMatchesByPlayerIdAsync(playerId);

        // Assert
        result.Should().HaveCount(2);
    }
}
