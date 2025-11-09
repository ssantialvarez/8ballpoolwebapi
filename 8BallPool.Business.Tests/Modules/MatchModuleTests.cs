using System.Security.Claims;
using _8BallPool.Business.DTOs;
using _8BallPool.Business.Modules;
using _8BallPool.Data.Exceptions;
using _8BallPool.Data.Interfaces;
using _8BallPool.Data.Models;
using FluentAssertions;
using Moq;
using MatchModel = _8BallPool.Data.Models.Match;

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

    #region GetMatchByIdAsync Tests

    [Fact]
    public async Task GetMatchByIdAsync_WithValidId_ReturnsMatchResponseDto()
    {
        // Arrange
        var matchId = 1;
        var match = new MatchModel
        {
            Id = matchId,
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow,
            EndTime = null,
            WinnerId = null,
            TableNumber = 5
        };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync(match);

        // Act
        var result = await _matchModule.GetMatchByIdAsync(matchId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(matchId);
        result.Player1Id.Should().Be(1);
        result.Player2Id.Should().Be(2);
        result.TableNumber.Should().Be(5);
        _mockMatchRepository.Verify(r => r.GetMatchByIdAsync(matchId), Times.Once);
    }

    [Fact]
    public async Task GetMatchByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var matchId = 999;

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync((MatchModel?)null);

        // Act
        var result = await _matchModule.GetMatchByIdAsync(matchId);

        // Assert
        result.Should().BeNull();
        _mockMatchRepository.Verify(r => r.GetMatchByIdAsync(matchId), Times.Once);
    }

    #endregion

    #region GetAllMatchesAsync Tests

    [Fact]
    public async Task GetAllMatchesAsync_ReturnsAllMatches()
    {
        // Arrange
        var matches = new List<MatchModel>
        {
            new MatchModel { Id = 1, Player1Id = 1, Player2Id = 2, StartTime = DateTime.UtcNow },
            new MatchModel { Id = 2, Player1Id = 3, Player2Id = 4, StartTime = DateTime.UtcNow.AddHours(-1) }
        };

        _mockMatchRepository.Setup(r => r.GetAllMatchesAsync())
            .ReturnsAsync(matches);

        // Act
        var result = await _matchModule.GetAllMatchesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(m => m.Id > 0);
        _mockMatchRepository.Verify(r => r.GetAllMatchesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllMatchesAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        _mockMatchRepository.Setup(r => r.GetAllMatchesAsync())
            .ReturnsAsync(new List<MatchModel>());

        // Act
        var result = await _matchModule.GetAllMatchesAsync();

        // Assert
        result.Should().BeEmpty();
        _mockMatchRepository.Verify(r => r.GetAllMatchesAsync(), Times.Once);
    }

    #endregion

    #region GetMatchesByPlayerIdAsync Tests

    [Fact]
    public async Task GetMatchesByPlayerIdAsync_WithValidPlayerId_ReturnsMatches()
    {
        // Arrange
        var playerId = 1;
        var player = new Player 
        { 
            Id = playerId, 
            Auth0_id = "auth0|1", 
            Name = "Player 1", 
            Ranking = 1500,
            Profile_picture_url = "pic.jpg"
        };

        var matches = new List<MatchModel>
        {
            new MatchModel { Id = 1, Player1Id = playerId, Player2Id = 2, StartTime = DateTime.UtcNow },
            new MatchModel { Id = 2, Player1Id = 3, Player2Id = playerId, StartTime = DateTime.UtcNow.AddHours(-1) }
        };

        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
            .ReturnsAsync(player);

        _mockMatchRepository.Setup(r => r.GetMatchesByPlayerIdAsync(playerId))
            .ReturnsAsync(matches);

        // Act
        var result = await _matchModule.GetMatchesByPlayerIdAsync(playerId);

        // Assert
        result.Should().HaveCount(2);
        _mockPlayerRepository.Verify(r => r.GetPlayerByIdAsync(playerId), Times.Once);
        _mockMatchRepository.Verify(r => r.GetMatchesByPlayerIdAsync(playerId), Times.Once);
    }

    [Fact]
    public async Task GetMatchesByPlayerIdAsync_WithInvalidPlayerId_ThrowsArgumentException()
    {
        // Arrange
        var playerId = 999;

        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
            .ReturnsAsync((Player?)null);

        // Act
        var act = async () => await _matchModule.GetMatchesByPlayerIdAsync(playerId);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Player does not exist.");
        _mockMatchRepository.Verify(r => r.GetMatchesByPlayerIdAsync(It.IsAny<int>()), Times.Never);
    }

    #endregion

    #region AddMatchAsync Tests

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

        var player1 = new Player 
        { 
            Id = 1, 
            Auth0_id = "auth0|1", 
            Name = "Player 1", 
            Ranking = 1500,
            Profile_picture_url = "pic1.jpg"
        };
        
        var player2 = new Player 
        { 
            Id = 2, 
            Auth0_id = "auth0|2", 
            Name = "Player 2", 
            Ranking = 1600,
            Profile_picture_url = "pic2.jpg"
        };

        var createdMatch = new MatchModel
        {
            Id = 1,
            Player1Id = matchDto.Player1Id,
            Player2Id = matchDto.Player2Id,
            StartTime = matchDto.StartTime
        };

        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(1)).ReturnsAsync(player1);
        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(2)).ReturnsAsync(player2);
        _mockMatchRepository.Setup(r => r.HasOverlappingMatchAsync(1, matchDto.StartTime, null))
            .ReturnsAsync(false);
        _mockMatchRepository.Setup(r => r.HasOverlappingMatchAsync(2, matchDto.StartTime, null))
            .ReturnsAsync(false);
        _mockMatchRepository.Setup(r => r.AddMatchAsync(It.IsAny<MatchModel>()))
            .ReturnsAsync(createdMatch);

        // Act
        var result = await _matchModule.AddMatchAsync(matchDto);

        // Assert
        result.Should().NotBeNull();
        result!.Player1Id.Should().Be(1);
        result.Player2Id.Should().Be(2);
        _mockMatchRepository.Verify(r => r.AddMatchAsync(It.IsAny<MatchModel>()), Times.Once);
    }

    [Fact]
    public async Task AddMatchAsync_WithNonExistentPlayer1_ThrowsArgumentException()
    {
        // Arrange
        var matchDto = new MatchDto
        {
            Player1Id = 999,
            Player2Id = 2,
            StartTime = DateTime.UtcNow
        };

        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(999))
            .ReturnsAsync((Player?)null);
        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(2))
            .ReturnsAsync(new Player 
            { 
                Id = 2, 
                Auth0_id = "auth0|2", 
                Name = "Player 2", 
                Ranking = 1600,
                Profile_picture_url = "pic.jpg"
            });

        // Act
        var act = async () => await _matchModule.AddMatchAsync(matchDto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("One or both players do not exist.");
        _mockMatchRepository.Verify(r => r.AddMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    [Fact]
    public async Task AddMatchAsync_WithNonExistentPlayer2_ThrowsArgumentException()
    {
        // Arrange
        var matchDto = new MatchDto
        {
            Player1Id = 1,
            Player2Id = 999,
            StartTime = DateTime.UtcNow
        };

        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(1))
            .ReturnsAsync(new Player 
            { 
                Id = 1, 
                Auth0_id = "auth0|1", 
                Name = "Player 1", 
                Ranking = 1500,
                Profile_picture_url = "pic.jpg"
            });
        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(999))
            .ReturnsAsync((Player?)null);

        // Act
        var act = async () => await _matchModule.AddMatchAsync(matchDto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("One or both players do not exist.");
        _mockMatchRepository.Verify(r => r.AddMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    [Fact]
    public async Task AddMatchAsync_WithSamePlayer_ThrowsArgumentException()
    {
        // Arrange
        var matchDto = new MatchDto
        {
            Player1Id = 1,
            Player2Id = 1,
            StartTime = DateTime.UtcNow
        };

        var player = new Player 
        { 
            Id = 1, 
            Auth0_id = "auth0|1", 
            Name = "Player 1", 
            Ranking = 1500,
            Profile_picture_url = "pic.jpg"
        };

        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(1))
            .ReturnsAsync(player);

        // Act
        var act = async () => await _matchModule.AddMatchAsync(matchDto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("A player cannot play against themselves.");
        _mockMatchRepository.Verify(r => r.AddMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    [Fact]
    public async Task AddMatchAsync_WithPlayer1DoubleBooking_ThrowsDoubleBookingException()
    {
        // Arrange
        var matchDto = new MatchDto
        {
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow
        };

        var player1 = new Player 
        { 
            Id = 1, 
            Auth0_id = "auth0|1", 
            Name = "Player 1", 
            Ranking = 1500,
            Profile_picture_url = "pic1.jpg"
        };
        
        var player2 = new Player 
        { 
            Id = 2, 
            Auth0_id = "auth0|2", 
            Name = "Player 2", 
            Ranking = 1600,
            Profile_picture_url = "pic2.jpg"
        };

        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(1)).ReturnsAsync(player1);
        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(2)).ReturnsAsync(player2);
        _mockMatchRepository.Setup(r => r.HasOverlappingMatchAsync(1, matchDto.StartTime, null))
            .ReturnsAsync(true); // Player 1 has overlapping match

        // Act
        var act = async () => await _matchModule.AddMatchAsync(matchDto);

        // Assert
        await act.Should().ThrowAsync<DoubleBookingException>()
            .WithMessage("Player 1 already has a match scheduled during this time.");
        _mockMatchRepository.Verify(r => r.AddMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    [Fact]
    public async Task AddMatchAsync_WithPlayer2DoubleBooking_ThrowsDoubleBookingException()
    {
        // Arrange
        var matchDto = new MatchDto
        {
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow
        };

        var player1 = new Player 
        { 
            Id = 1, 
            Auth0_id = "auth0|1", 
            Name = "Player 1", 
            Ranking = 1500,
            Profile_picture_url = "pic1.jpg"
        };
        
        var player2 = new Player 
        { 
            Id = 2, 
            Auth0_id = "auth0|2", 
            Name = "Player 2", 
            Ranking = 1600,
            Profile_picture_url = "pic2.jpg"
        };

        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(1)).ReturnsAsync(player1);
        _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(2)).ReturnsAsync(player2);
        _mockMatchRepository.Setup(r => r.HasOverlappingMatchAsync(1, matchDto.StartTime, null))
            .ReturnsAsync(false);
        _mockMatchRepository.Setup(r => r.HasOverlappingMatchAsync(2, matchDto.StartTime, null))
            .ReturnsAsync(true); // Player 2 has overlapping match

        // Act
        var act = async () => await _matchModule.AddMatchAsync(matchDto);

        // Assert
        await act.Should().ThrowAsync<DoubleBookingException>()
            .WithMessage("Player 2 already has a match scheduled during this time.");
        _mockMatchRepository.Verify(r => r.AddMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    #endregion

    #region UpdateMatchAsync Tests

    [Fact]
    public async Task UpdateMatchAsync_WithValidTableNumber_UpdatesMatch()
    {
        // Arrange
        var matchId = 1;
        var existingMatch = new MatchModel
        {
            Id = matchId,
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow,
            TableNumber = 5
        };

        var updateDto = new UpdateMatchDto
        {
            TableNumber = 10
        };

        var updatedMatch = new MatchModel
        {
            Id = matchId,
            Player1Id = 1,
            Player2Id = 2,
            StartTime = existingMatch.StartTime,
            TableNumber = 10
        };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync(existingMatch);
        _mockMatchRepository.Setup(r => r.UpdateMatchAsync(It.IsAny<MatchModel>()))
            .ReturnsAsync(updatedMatch);

        // Act
        var result = await _matchModule.UpdateMatchAsync(matchId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.TableNumber.Should().Be(10);
        _mockMatchRepository.Verify(r => r.UpdateMatchAsync(It.IsAny<MatchModel>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMatchAsync_WithNonExistentMatch_ThrowsArgumentException()
    {
        // Arrange
        var matchId = 999;
        var updateDto = new UpdateMatchDto { TableNumber = 10 };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync((MatchModel?)null);

        // Act
        var act = async () => await _matchModule.UpdateMatchAsync(matchId, updateDto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Match does not exist.");
        _mockMatchRepository.Verify(r => r.UpdateMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    #endregion

    #region FinishMatchAsync Tests

    [Fact]
    public async Task FinishMatchAsync_AsParticipant_FinishesMatch()
    {
        // Arrange
        var matchId = 1;
        var winnerId = 1;
        var auth0Id = "auth0|1";

        var match = new MatchModel
        {
            Id = matchId,
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = null
        };

        var player = new Player
        {
            Id = 1,
            Auth0_id = auth0Id,
            Name = "Player 1",
            Ranking = 1500,
            Profile_picture_url = "pic.jpg"
        };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync(match);
        _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
            .ReturnsAsync(player);
        _mockMatchRepository.Setup(r => r.UpdateMatchAsync(It.IsAny<MatchModel>()))
            .ReturnsAsync(match);

        // Act
        var result = await _matchModule.FinishMatchAsync(matchId, winnerId, auth0Id, false);

        // Assert
        result.Should().NotBeNull();
        result!.WinnerId.Should().Be(winnerId);
        result.EndTime.Should().NotBeNull();
        _mockMatchRepository.Verify(r => r.UpdateMatchAsync(It.IsAny<MatchModel>()), Times.Once);
    }

    [Fact]
    public async Task FinishMatchAsync_AsAdmin_FinishesMatchWithoutParticipantCheck()
    {
        // Arrange
        var matchId = 1;
        var winnerId = 1;
        var auth0Id = "auth0|admin";

        var match = new MatchModel
        {
            Id = matchId,
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = null
        };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync(match);
        _mockMatchRepository.Setup(r => r.UpdateMatchAsync(It.IsAny<MatchModel>()))
            .ReturnsAsync(match);

        // Act
        var result = await _matchModule.FinishMatchAsync(matchId, winnerId, auth0Id, true);

        // Assert
        result.Should().NotBeNull();
        result!.WinnerId.Should().Be(winnerId);
        _mockPlayerRepository.Verify(r => r.GetPlayerByAuth0IdAsync(It.IsAny<string>()), Times.Never);
        _mockMatchRepository.Verify(r => r.UpdateMatchAsync(It.IsAny<MatchModel>()), Times.Once);
    }

    [Fact]
    public async Task FinishMatchAsync_WithNonExistentMatch_ThrowsArgumentException()
    {
        // Arrange
        var matchId = 999;
        var winnerId = 1;
        var auth0Id = "auth0|1";

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync((MatchModel?)null);

        // Act
        var act = async () => await _matchModule.FinishMatchAsync(matchId, winnerId, auth0Id, false);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Match does not exist.");
        _mockMatchRepository.Verify(r => r.UpdateMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    [Fact]
    public async Task FinishMatchAsync_WithAlreadyFinishedMatch_ThrowsInvalidOperationException()
    {
        // Arrange
        var matchId = 1;
        var winnerId = 1;
        var auth0Id = "auth0|1";

        var match = new MatchModel
        {
            Id = matchId,
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow.AddHours(-2),
            EndTime = DateTime.UtcNow.AddHours(-1),
            WinnerId = 1
        };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync(match);

        // Act
        var act = async () => await _matchModule.FinishMatchAsync(matchId, winnerId, auth0Id, false);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Match is already finished.");
        _mockMatchRepository.Verify(r => r.UpdateMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    [Fact]
    public async Task FinishMatchAsync_WithInvalidWinner_ThrowsArgumentException()
    {
        // Arrange
        var matchId = 1;
        var winnerId = 999; // Not a participant
        var auth0Id = "auth0|1";

        var match = new MatchModel
        {
            Id = matchId,
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = null
        };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync(match);

        // Act
        var act = async () => await _matchModule.FinishMatchAsync(matchId, winnerId, auth0Id, false);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Winner must be one of the players in the match.");
        _mockMatchRepository.Verify(r => r.UpdateMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    [Fact]
    public async Task FinishMatchAsync_AsNonParticipant_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var matchId = 1;
        var winnerId = 1;
        var auth0Id = "auth0|999"; // Not a participant

        var match = new MatchModel
        {
            Id = matchId,
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = null
        };

        var player = new Player
        {
            Id = 999,
            Auth0_id = auth0Id,
            Name = "Non Participant",
            Ranking = 1500,
            Profile_picture_url = "pic.jpg"
        };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync(match);
        _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
            .ReturnsAsync(player);

        // Act
        var act = async () => await _matchModule.FinishMatchAsync(matchId, winnerId, auth0Id, false);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("You are not authorized to finish this match.");
        _mockMatchRepository.Verify(r => r.UpdateMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    [Fact]
    public async Task FinishMatchAsync_WithNonExistentPlayer_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var matchId = 1;
        var winnerId = 1;
        var auth0Id = "auth0|999";

        var match = new MatchModel
        {
            Id = matchId,
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = null
        };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync(match);
        _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
            .ReturnsAsync((Player?)null);

        // Act
        var act = async () => await _matchModule.FinishMatchAsync(matchId, winnerId, auth0Id, false);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Player not found.");
        _mockMatchRepository.Verify(r => r.UpdateMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    #endregion

    #region DeleteMatchAsync Tests

    [Fact]
    public async Task DeleteMatchAsync_AsParticipant_DeletesMatch()
    {
        // Arrange
        var matchId = 1;
        var auth0Id = "auth0|1";

        var match = new MatchModel
        {
            Id = matchId,
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow
        };

        var player = new Player
        {
            Id = 1,
            Auth0_id = auth0Id,
            Name = "Player 1",
            Ranking = 1500,
            Profile_picture_url = "pic.jpg"
        };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync(match);
        _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
            .ReturnsAsync(player);
        _mockMatchRepository.Setup(r => r.DeleteMatchAsync(match))
            .ReturnsAsync(true);

        // Act
        var result = await _matchModule.DeleteMatchAsync(matchId, auth0Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(matchId);
        _mockMatchRepository.Verify(r => r.DeleteMatchAsync(match), Times.Once);
    }

    [Fact]
    public async Task DeleteMatchAsync_WithNonExistentMatch_ThrowsArgumentException()
    {
        // Arrange
        var matchId = 999;
        var auth0Id = "auth0|1";

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync((MatchModel?)null);

        // Act
        var act = async () => await _matchModule.DeleteMatchAsync(matchId, auth0Id);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Match does not exist.");
        _mockMatchRepository.Verify(r => r.DeleteMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    [Fact]
    public async Task DeleteMatchAsync_WithNonExistentPlayer_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var matchId = 1;
        var auth0Id = "auth0|999";

        var match = new MatchModel
        {
            Id = matchId,
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow
        };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync(match);
        _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
            .ReturnsAsync((Player?)null);

        // Act
        var act = async () => await _matchModule.DeleteMatchAsync(matchId, auth0Id);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Player not found.");
        _mockMatchRepository.Verify(r => r.DeleteMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    [Fact]
    public async Task DeleteMatchAsync_AsNonParticipant_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var matchId = 1;
        var auth0Id = "auth0|999";

        var match = new MatchModel
        {
            Id = matchId,
            Player1Id = 1,
            Player2Id = 2,
            StartTime = DateTime.UtcNow
        };

        var player = new Player
        {
            Id = 999,
            Auth0_id = auth0Id,
            Name = "Non Participant",
            Ranking = 1500,
            Profile_picture_url = "pic.jpg"
        };

        _mockMatchRepository.Setup(r => r.GetMatchByIdAsync(matchId))
            .ReturnsAsync(match);
        _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
            .ReturnsAsync(player);

        // Act
        var act = async () => await _matchModule.DeleteMatchAsync(matchId, auth0Id);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("You are not authorized to delete this match.");
        _mockMatchRepository.Verify(r => r.DeleteMatchAsync(It.IsAny<MatchModel>()), Times.Never);
    }

    #endregion
}
