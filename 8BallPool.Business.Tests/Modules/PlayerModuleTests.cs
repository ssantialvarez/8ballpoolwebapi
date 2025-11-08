using _8BallPool.Business.DTOs;
using _8BallPool.Business.Modules;
using _8BallPool.Data.Interfaces;
using _8BallPool.Data.Models;
using FluentAssertions;
using Moq;

namespace _8BallPool.Business.Tests.Modules
{
    public class PlayerModuleTests
    {
        private readonly Mock<IPlayerRepository> _mockPlayerRepository;
        private readonly PlayerModule _playerModule;

        public PlayerModuleTests()
        {
            _mockPlayerRepository = new Mock<IPlayerRepository>();
            _playerModule = new PlayerModule(_mockPlayerRepository.Object);
        }

        [Fact]
        public async Task CreatePlayerAsync_WithValidData_CreatesPlayer()
        {
            // Arrange
            var playerDto = new PlayerDto
            {
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1500,
                Preferred_cue = "Predator",
                Profile_picture_url = "https://example.com/pic.jpg"
            };

            _mockPlayerRepository.Setup(r => r.AddPlayerAsync(It.IsAny<Player>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _playerModule.CreatePlayerAsync(playerDto);

            // Assert
            result.Should().NotBeNull();
            result.Auth0_id.Should().Be(playerDto.Auth0_id);
            result.Name.Should().Be(playerDto.Name);
            result.Ranking.Should().Be(playerDto.Ranking);
            result.Preferred_cue.Should().Be(playerDto.Preferred_cue);
            result.Profile_picture_url.Should().Be(playerDto.Profile_picture_url);
            _mockPlayerRepository.Verify(r => r.AddPlayerAsync(It.IsAny<Player>()), Times.Once);
        }

        [Fact]
        public async Task RegisterPlayerAsync_WithNewPlayer_CreatesPlayer()
        {
            // Arrange
            var playerDto = new PlayerDto
            {
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1500,
                Preferred_cue = "Predator",
                Profile_picture_url = "https://example.com/pic.jpg"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(playerDto.Auth0_id))
                .ReturnsAsync((Player?)null);
            _mockPlayerRepository.Setup(r => r.AddPlayerAsync(It.IsAny<Player>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _playerModule.RegisterPlayerAsync(playerDto);

            // Assert
            result.Should().NotBeNull();
            result.Auth0_id.Should().Be(playerDto.Auth0_id);
            result.Name.Should().Be(playerDto.Name);
            _mockPlayerRepository.Verify(r => r.GetPlayerByAuth0IdAsync(playerDto.Auth0_id), Times.Once);
            _mockPlayerRepository.Verify(r => r.AddPlayerAsync(It.IsAny<Player>()), Times.Once);
        }

        [Fact]
        public async Task RegisterPlayerAsync_WithExistingPlayer_ReturnsExistingPlayer()
        {
            // Arrange
            var playerDto = new PlayerDto
            {
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1500,
                Profile_picture_url = "https://example.com/pic.jpg"
            };

            var existingPlayer = new Player
            {
                Id = 1,
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1600,
                Preferred_cue = "Old Cue",
                Profile_picture_url = "https://example.com/old.jpg"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(playerDto.Auth0_id))
                .ReturnsAsync(existingPlayer);

            // Act
            var result = await _playerModule.RegisterPlayerAsync(playerDto);

            // Assert
            result.Should().Be(existingPlayer);
            _mockPlayerRepository.Verify(r => r.GetPlayerByAuth0IdAsync(playerDto.Auth0_id), Times.Once);
            _mockPlayerRepository.Verify(r => r.AddPlayerAsync(It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public async Task GetPlayersAsync_WithoutFilter_ReturnsAllPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Id = 1, Auth0_id = "auth0|1", Name = "Alice", Ranking = 1500, Profile_picture_url = "url1" },
                new Player { Id = 2, Auth0_id = "auth0|2", Name = "Bob", Ranking = 1600, Profile_picture_url = "url2" },
                new Player { Id = 3, Auth0_id = "auth0|3", Name = "Charlie", Ranking = 1400, Profile_picture_url = "url3" }
            };

            _mockPlayerRepository.Setup(r => r.GetAllPlayersAsync())
                .ReturnsAsync(players);

            // Act
            var result = await _playerModule.GetPlayersAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(players);
        }

        [Fact]
        public async Task GetPlayersAsync_WithNameFilter_ReturnsFilteredPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Id = 1, Auth0_id = "auth0|1", Name = "Alice Smith", Ranking = 1500, Profile_picture_url = "url1" },
                new Player { Id = 2, Auth0_id = "auth0|2", Name = "Bob Jones", Ranking = 1600, Profile_picture_url = "url2" },
                new Player { Id = 3, Auth0_id = "auth0|3", Name = "Alice Johnson", Ranking = 1400, Profile_picture_url = "url3" }
            };

            _mockPlayerRepository.Setup(r => r.GetAllPlayersAsync())
                .ReturnsAsync(players);

            // Act
            var result = await _playerModule.GetPlayersAsync("Alice");

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(p => p.Name.Contains("Alice", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetPlayerByIdAsync_WhenPlayerExists_ReturnsPlayer()
        {
            // Arrange
            var playerId = 1;
            var player = new Player
            {
                Id = playerId,
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1500,
                Profile_picture_url = "url"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync(player);

            // Act
            var result = await _playerModule.GetPlayerByIdAsync(playerId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(player);
        }

        [Fact]
        public async Task GetPlayerByIdAsync_WhenPlayerDoesNotExist_ReturnsNull()
        {
            // Arrange
            var playerId = 999;

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync((Player?)null);

            // Act
            var result = await _playerModule.GetPlayerByIdAsync(playerId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetPlayerByAuth0IdAsync_WhenPlayerExists_ReturnsPlayer()
        {
            // Arrange
            var auth0Id = "auth0|123";
            var player = new Player
            {
                Id = 1,
                Auth0_id = auth0Id,
                Name = "John Doe",
                Ranking = 1500,
                Profile_picture_url = "url"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
                .ReturnsAsync(player);

            // Act
            var result = await _playerModule.GetPlayerByAuth0IdAsync(auth0Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(player);
        }

        [Fact]
        public async Task GetPlayerByAuth0IdAsync_WhenPlayerDoesNotExist_ReturnsNull()
        {
            // Arrange
            var auth0Id = "auth0|999";

            _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
                .ReturnsAsync((Player?)null);

            // Act
            var result = await _playerModule.GetPlayerByAuth0IdAsync(auth0Id);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdatePlayerAsync_WhenPlayerExists_UpdatesAndReturnsPlayer()
        {
            // Arrange
            var playerId = 1;
            var existingPlayer = new Player
            {
                Id = playerId,
                Auth0_id = "auth0|123",
                Name = "Old Name",
                Ranking = 1500,
                Preferred_cue = "Old Cue",
                Profile_picture_url = "old_url"
            };

            var updatedPlayer = new Player
            {
                Auth0_id = "auth0|456",
                Name = "New Name",
                Ranking = 1700,
                Profile_picture_url = "new_url"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync(existingPlayer);
            _mockPlayerRepository.Setup(r => r.UpdatePlayerAsync(It.IsAny<Player>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _playerModule.UpdatePlayerAsync(playerId, updatedPlayer);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("New Name");
            result.Ranking.Should().Be(1700);
            result.Auth0_id.Should().Be("auth0|123"); // Should not change
            _mockPlayerRepository.Verify(r => r.UpdatePlayerAsync(It.IsAny<Player>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePlayerAsync_WhenPlayerDoesNotExist_ReturnsNull()
        {
            // Arrange
            var playerId = 999;
            var updatedPlayer = new Player
            {
                Auth0_id = "auth0|456",
                Name = "New Name",
                Ranking = 1700,
                Profile_picture_url = "new_url"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync((Player?)null);

            // Act
            var result = await _playerModule.UpdatePlayerAsync(playerId, updatedPlayer);

            // Assert
            result.Should().BeNull();
            _mockPlayerRepository.Verify(r => r.UpdatePlayerAsync(It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePlayerMeAsync_WhenPlayerExists_UpdatesAndReturnsPlayer()
        {
            // Arrange
            var auth0Id = "auth0|123";
            var existingPlayer = new Player
            {
                Id = 1,
                Auth0_id = auth0Id,
                Name = "Old Name",
                Ranking = 1500,
                Preferred_cue = "Old Cue",
                Profile_picture_url = "old_url"
            };

            var updatedPlayer = new Player
            {
                Auth0_id = "auth0|456",
                Name = "New Name",
                Ranking = 1700,
                Profile_picture_url = "new_url"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
                .ReturnsAsync(existingPlayer);
            _mockPlayerRepository.Setup(r => r.UpdatePlayerAsync(It.IsAny<Player>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _playerModule.UpdatePlayerMeAsync(auth0Id, updatedPlayer);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("New Name");
            result.Ranking.Should().Be(1700);
            result.Auth0_id.Should().Be(auth0Id); // Should not change
            _mockPlayerRepository.Verify(r => r.UpdatePlayerAsync(It.IsAny<Player>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePlayerMeAsync_WhenPlayerDoesNotExist_ReturnsNull()
        {
            // Arrange
            var auth0Id = "auth0|999";
            var updatedPlayer = new Player
            {
                Auth0_id = "auth0|456",
                Name = "New Name",
                Ranking = 1700,
                Profile_picture_url = "new_url"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
                .ReturnsAsync((Player?)null);

            // Act
            var result = await _playerModule.UpdatePlayerMeAsync(auth0Id, updatedPlayer);

            // Assert
            result.Should().BeNull();
            _mockPlayerRepository.Verify(r => r.UpdatePlayerAsync(It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public async Task DeletePlayerAsync_WhenPlayerExists_DeletesAndReturnsTrue()
        {
            // Arrange
            var playerId = 1;
            var existingPlayer = new Player
            {
                Id = playerId,
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1500,
                Profile_picture_url = "url"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync(existingPlayer);
            _mockPlayerRepository.Setup(r => r.DeletePlayerAsync(playerId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _playerModule.DeletePlayerAsync(playerId);

            // Assert
            result.Should().BeTrue();
            _mockPlayerRepository.Verify(r => r.DeletePlayerAsync(playerId), Times.Once);
        }

        [Fact]
        public async Task DeletePlayerAsync_WhenPlayerDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var playerId = 999;

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync((Player?)null);

            // Act
            var result = await _playerModule.DeletePlayerAsync(playerId);

            // Assert
            result.Should().BeFalse();
            _mockPlayerRepository.Verify(r => r.DeletePlayerAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
