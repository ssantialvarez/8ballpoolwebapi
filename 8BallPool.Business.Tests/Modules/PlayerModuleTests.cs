using System.Security.Claims;
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

        #region CreatePlayerAsync Tests

        [Fact]
        public async Task CreatePlayerAsync_WithValidPlayer_ReturnsCreatedPlayer()
        {
            // Arrange
            var playerDto = new PlayerDto
            {
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1500,
                Preferred_cue = "Predator",
                Profile_picture = "https://example.com/pic.jpg"
            };

            var expectedPlayer = new Player
            {
                Id = 1,
                Auth0_id = playerDto.Auth0_id,
                Name = playerDto.Name,
                Ranking = playerDto.Ranking,
                Preferred_cue = playerDto.Preferred_cue,
                Profile_picture_url = playerDto.Profile_picture
            };

            _mockPlayerRepository.Setup(r => r.AddPlayerAsync(It.IsAny<Player>()))
                .ReturnsAsync(expectedPlayer);

            // Act
            var result = await _playerModule.CreatePlayerAsync(playerDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedPlayer.Id);
            result.Auth0_id.Should().Be(playerDto.Auth0_id);
            result.Name.Should().Be(playerDto.Name);
            result.Ranking.Should().Be(playerDto.Ranking);
            _mockPlayerRepository.Verify(r => r.AddPlayerAsync(It.IsAny<Player>()), Times.Once);
        }

        [Fact]
        public async Task CreatePlayerAsync_WithDuplicateAuth0Id_ThrowsDuplicatePlayerException()
        {
            // Arrange
            var playerDto = new PlayerDto
            {
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1500,
                Preferred_cue = "Predator",
                Profile_picture = "https://example.com/pic.jpg"
            };

            _mockPlayerRepository.Setup(r => r.AddPlayerAsync(It.IsAny<Player>()))
                .ThrowsAsync(new DuplicatePlayerException("Player already exists.", new Exception()));

            // Act
            var act = async () => await _playerModule.CreatePlayerAsync(playerDto);

            // Assert
            await act.Should().ThrowAsync<DuplicatePlayerException>();
            _mockPlayerRepository.Verify(r => r.AddPlayerAsync(It.IsAny<Player>()), Times.Once);
        }

        #endregion

        #region GetPlayersAsync Tests

        [Fact]
        public async Task GetPlayersAsync_WithoutFilter_ReturnsAllPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Id = 1, Auth0_id = "auth0|1", Name = "John Doe", Ranking = 1500, Profile_picture_url = "pic1.jpg" },
                new Player { Id = 2, Auth0_id = "auth0|2", Name = "Jane Smith", Ranking = 1600, Profile_picture_url = "pic2.jpg" }
            };

            _mockPlayerRepository.Setup(r => r.GetAllPlayersAsync())
                .ReturnsAsync(players);

            // Act
            var result = await _playerModule.GetPlayersAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(players);
            _mockPlayerRepository.Verify(r => r.GetAllPlayersAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPlayersAsync_WithNameFilter_ReturnsFilteredPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Id = 1, Auth0_id = "auth0|1", Name = "John Doe", Ranking = 1500, Profile_picture_url = "pic1.jpg" },
                new Player { Id = 2, Auth0_id = "auth0|2", Name = "Jane Smith", Ranking = 1600, Profile_picture_url = "pic2.jpg" },
                new Player { Id = 3, Auth0_id = "auth0|3", Name = "Bob Johnson", Ranking = 1400, Profile_picture_url = "pic3.jpg" }
            };

            _mockPlayerRepository.Setup(r => r.GetAllPlayersAsync())
                .ReturnsAsync(players);

            // Act
            var result = await _playerModule.GetPlayersAsync("John");

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Name == "John Doe");
            result.Should().Contain(p => p.Name == "Bob Johnson");
            _mockPlayerRepository.Verify(r => r.GetAllPlayersAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPlayersAsync_WithNonMatchingFilter_ReturnsEmptyList()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Id = 1, Auth0_id = "auth0|1", Name = "John Doe", Ranking = 1500, Profile_picture_url = "pic1.jpg" }
            };

            _mockPlayerRepository.Setup(r => r.GetAllPlayersAsync())
                .ReturnsAsync(players);

            // Act
            var result = await _playerModule.GetPlayersAsync("NonExistent");

            // Assert
            result.Should().BeEmpty();
            _mockPlayerRepository.Verify(r => r.GetAllPlayersAsync(), Times.Once);
        }

        #endregion

        #region RegisterPlayerAsync Tests

        [Fact]
        public async Task RegisterPlayerAsync_WithNewPlayer_CreatesAndReturnsPlayer()
        {
            // Arrange
            var playerDto = new PlayerDto
            {
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1500,
                Preferred_cue = "Predator",
                Profile_picture = "https://example.com/pic.jpg"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(playerDto.Auth0_id))
                .ReturnsAsync((Player?)null);

            var expectedPlayer = new Player
            {
                Id = 1,
                Auth0_id = playerDto.Auth0_id,
                Name = playerDto.Name,
                Ranking = playerDto.Ranking,
                Preferred_cue = playerDto.Preferred_cue,
                Profile_picture_url = playerDto.Profile_picture
            };

            _mockPlayerRepository.Setup(r => r.AddPlayerAsync(It.IsAny<Player>()))
                .ReturnsAsync(expectedPlayer);

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
                Preferred_cue = "Predator",
                Profile_picture = "https://example.com/pic.jpg"
            };

            var existingPlayer = new Player
            {
                Id = 1,
                Auth0_id = playerDto.Auth0_id,
                Name = "Existing Name",
                Ranking = 1600,
                Profile_picture_url = "existing.jpg"
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

        #endregion

        #region GetPlayerByIdAsync Tests

        [Fact]
        public async Task GetPlayerByIdAsync_WithValidId_ReturnsPlayer()
        {
            // Arrange
            var playerId = 1;
            var expectedPlayer = new Player
            {
                Id = playerId,
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1500,
                Profile_picture_url = "pic.jpg"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync(expectedPlayer);

            // Act
            var result = await _playerModule.GetPlayerByIdAsync(playerId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedPlayer);
            _mockPlayerRepository.Verify(r => r.GetPlayerByIdAsync(playerId), Times.Once);
        }

        [Fact]
        public async Task GetPlayerByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var playerId = 999;

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync((Player?)null);

            // Act
            var result = await _playerModule.GetPlayerByIdAsync(playerId);

            // Assert
            result.Should().BeNull();
            _mockPlayerRepository.Verify(r => r.GetPlayerByIdAsync(playerId), Times.Once);
        }

        #endregion

        #region GetPlayerByAuth0IdAsync Tests

        [Fact]
        public async Task GetPlayerByAuth0IdAsync_WithValidAuth0Id_ReturnsPlayer()
        {
            // Arrange
            var auth0Id = "auth0|123";
            var expectedPlayer = new Player
            {
                Id = 1,
                Auth0_id = auth0Id,
                Name = "John Doe",
                Ranking = 1500,
                Profile_picture_url = "pic.jpg"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, auth0Id)
            };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
                .ReturnsAsync(expectedPlayer);

            // Act
            var result = await _playerModule.GetPlayerByAuth0IdAsync(user);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedPlayer);
            _mockPlayerRepository.Verify(r => r.GetPlayerByAuth0IdAsync(auth0Id), Times.Once);
        }

        [Fact]
        public async Task GetPlayerByAuth0IdAsync_WithMissingAuth0IdClaim_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var claims = new List<Claim>();
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            // Act
            var act = async () => await _playerModule.GetPlayerByAuth0IdAsync(user);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Auth0 ID not found in user claims");
            _mockPlayerRepository.Verify(r => r.GetPlayerByAuth0IdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetPlayerByAuth0IdAsync_WithNonExistentPlayer_ReturnsNull()
        {
            // Arrange
            var auth0Id = "auth0|999";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, auth0Id)
            };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
                .ReturnsAsync((Player?)null);

            // Act
            var result = await _playerModule.GetPlayerByAuth0IdAsync(user);

            // Assert
            result.Should().BeNull();
            _mockPlayerRepository.Verify(r => r.GetPlayerByAuth0IdAsync(auth0Id), Times.Once);
        }

        #endregion

        #region UpdatePlayerAsync Tests

        [Fact]
        public async Task UpdatePlayerAsync_WithValidId_UpdatesAndReturnsPlayer()
        {
            // Arrange
            var playerId = 1;
            var existingPlayer = new Player
            {
                Id = playerId,
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1500,
                Preferred_cue = "Old Cue",
                Profile_picture_url = "old.jpg"
            };

            var updateDto = new UpdatePlayerDto
            {
                Name = "John Updated",
                Ranking = 1600,
                Preferred_cue = "New Cue",
                Profile_picture = "new.jpg"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync(existingPlayer);

            _mockPlayerRepository.Setup(r => r.UpdatePlayerAsync(It.IsAny<Player>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _playerModule.UpdatePlayerAsync(playerId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(updateDto.Name);
            result.Ranking.Should().Be(updateDto.Ranking.Value);
            result.Preferred_cue.Should().Be(updateDto.Preferred_cue);
            result.Profile_picture_url.Should().Be(updateDto.Profile_picture);
            _mockPlayerRepository.Verify(r => r.UpdatePlayerAsync(It.IsAny<Player>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePlayerAsync_WithPartialUpdate_UpdatesOnlyProvidedFields()
        {
            // Arrange
            var playerId = 1;
            var existingPlayer = new Player
            {
                Id = playerId,
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1500,
                Preferred_cue = "Old Cue",
                Profile_picture_url = "old.jpg"
            };

            var updateDto = new UpdatePlayerDto
            {
                Name = "John Updated"
                // Other fields are null
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync(existingPlayer);

            _mockPlayerRepository.Setup(r => r.UpdatePlayerAsync(It.IsAny<Player>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _playerModule.UpdatePlayerAsync(playerId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(updateDto.Name);
            result.Ranking.Should().Be(existingPlayer.Ranking); // Unchanged
            result.Preferred_cue.Should().Be(existingPlayer.Preferred_cue); // Unchanged
            result.Profile_picture_url.Should().Be(existingPlayer.Profile_picture_url); // Unchanged
        }

        [Fact]
        public async Task UpdatePlayerAsync_WithInvalidId_ThrowsArgumentException()
        {
            // Arrange
            var playerId = 999;
            var updateDto = new UpdatePlayerDto { Name = "Test" };

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync((Player?)null);

            // Act
            var act = async () => await _playerModule.UpdatePlayerAsync(playerId, updateDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Player does not exist.");
            _mockPlayerRepository.Verify(r => r.UpdatePlayerAsync(It.IsAny<Player>()), Times.Never);
        }

        #endregion

        #region UpdatePlayerMeAsync Tests

        [Fact]
        public async Task UpdatePlayerMeAsync_WithValidUser_UpdatesAndReturnsPlayer()
        {
            // Arrange
            var auth0Id = "auth0|123";
            var existingPlayer = new Player
            {
                Id = 1,
                Auth0_id = auth0Id,
                Name = "John Doe",
                Ranking = 1500,
                Profile_picture_url = "pic.jpg"
            };

            var updateDto = new UpdatePlayerDto
            {
                Name = "John Updated",
                Ranking = 1600
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, auth0Id)
            };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
                .ReturnsAsync(existingPlayer);

            _mockPlayerRepository.Setup(r => r.UpdatePlayerAsync(It.IsAny<Player>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _playerModule.UpdatePlayerMeAsync(user, updateDto);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(updateDto.Name);
            result.Ranking.Should().Be(updateDto.Ranking.Value);
            _mockPlayerRepository.Verify(r => r.UpdatePlayerAsync(It.IsAny<Player>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePlayerMeAsync_WithMissingAuth0Id_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var updateDto = new UpdatePlayerDto { Name = "Test" };
            var claims = new List<Claim>();
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            // Act
            var act = async () => await _playerModule.UpdatePlayerMeAsync(user, updateDto);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Auth0 ID not found in user claims");
            _mockPlayerRepository.Verify(r => r.UpdatePlayerAsync(It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePlayerMeAsync_WithNonExistentPlayer_ThrowsArgumentException()
        {
            // Arrange
            var auth0Id = "auth0|999";
            var updateDto = new UpdatePlayerDto { Name = "Test" };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, auth0Id)
            };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            _mockPlayerRepository.Setup(r => r.GetPlayerByAuth0IdAsync(auth0Id))
                .ReturnsAsync((Player?)null);

            // Act
            var act = async () => await _playerModule.UpdatePlayerMeAsync(user, updateDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Player does not exist.");
            _mockPlayerRepository.Verify(r => r.UpdatePlayerAsync(It.IsAny<Player>()), Times.Never);
        }

        #endregion

        #region DeletePlayerAsync Tests

        [Fact]
        public async Task DeletePlayerAsync_WithValidId_DeletesAndReturnsPlayer()
        {
            // Arrange
            var playerId = 1;
            var existingPlayer = new Player
            {
                Id = playerId,
                Auth0_id = "auth0|123",
                Name = "John Doe",
                Ranking = 1500,
                Profile_picture_url = "pic.jpg"
            };

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync(existingPlayer);

            _mockPlayerRepository.Setup(r => r.DeletePlayerAsync(existingPlayer))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _playerModule.DeletePlayerAsync(playerId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(existingPlayer);
            _mockPlayerRepository.Verify(r => r.DeletePlayerAsync(existingPlayer), Times.Once);
        }

        [Fact]
        public async Task DeletePlayerAsync_WithInvalidId_ThrowsArgumentException()
        {
            // Arrange
            var playerId = 999;

            _mockPlayerRepository.Setup(r => r.GetPlayerByIdAsync(playerId))
                .ReturnsAsync((Player?)null);

            // Act
            var act = async () => await _playerModule.DeletePlayerAsync(playerId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Player does not exist.");
            _mockPlayerRepository.Verify(r => r.DeletePlayerAsync(It.IsAny<Player>()), Times.Never);
        }

        #endregion
    }
}
