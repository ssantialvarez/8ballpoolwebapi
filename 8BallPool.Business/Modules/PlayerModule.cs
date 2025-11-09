using System.Security.Claims;
using _8BallPool.Business.DTOs;
using _8BallPool.Business.Interfaces;
using _8BallPool.Data.Interfaces;
using _8BallPool.Data.Models;

namespace _8BallPool.Business.Modules
{
    public class PlayerModule : IPlayerModule
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerModule(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<Player> CreatePlayerAsync(PlayerDto player)
        {
            var newPlayer = await _playerRepository.AddPlayerAsync(player.MapToPlayer());
            return newPlayer;
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync(string? nameFilter = null)
        {
            var players = await _playerRepository.GetAllPlayersAsync();
            if (string.IsNullOrEmpty(nameFilter))
            {
                return players;
            }
            List<Player> value = [.. players.Where(p => p.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))];
            players = value;
            return players;
        }

        public async Task<Player> RegisterPlayerAsync(PlayerDto player)
        {
            var existingPlayer = await _playerRepository.GetPlayerByAuth0IdAsync(player.Auth0_id);
            if (existingPlayer != null)
            {
                return existingPlayer;
            }

            var newPlayer = new Player
            {
                Auth0_id = player.Auth0_id,
                Name = player.Name,
                Ranking = player.Ranking,
                Preferred_cue = player.Preferred_cue,
                Profile_picture_url = player.Profile_picture
            };

            await _playerRepository.AddPlayerAsync(newPlayer);
            return newPlayer;
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _playerRepository.GetPlayerByIdAsync(id);
        }

        public async Task<Player?> GetPlayerByAuth0IdAsync(ClaimsPrincipal user)
        {
            var auth0Id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (auth0Id == null)
            {
                throw new UnauthorizedAccessException("Auth0 ID not found in user claims");
            }
            
            return await _playerRepository.GetPlayerByAuth0IdAsync(auth0Id);
        }

        public async Task<Player?> UpdatePlayerAsync(int id, UpdatePlayerDto updatedPlayer)
        {
            var existingPlayer = await _playerRepository.GetPlayerByIdAsync(id);
            if (existingPlayer == null)
            {
                throw new ArgumentException("Player does not exist.");
            }

            existingPlayer.Name = updatedPlayer.Name ?? existingPlayer.Name;
            existingPlayer.Ranking = updatedPlayer.Ranking ?? existingPlayer.Ranking;
            existingPlayer.Preferred_cue = updatedPlayer.Preferred_cue ?? existingPlayer.Preferred_cue;
            existingPlayer.Profile_picture_url = updatedPlayer.Profile_picture ?? existingPlayer.Profile_picture_url;

            await _playerRepository.UpdatePlayerAsync(existingPlayer);
            return existingPlayer;
        }

        public async Task<Player?> UpdatePlayerMeAsync(ClaimsPrincipal user, UpdatePlayerDto updatedPlayer)
        {
            var auth0Id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (auth0Id == null)
            {
                throw new UnauthorizedAccessException("Auth0 ID not found in user claims");
            }
            
            var existingPlayer = await _playerRepository.GetPlayerByAuth0IdAsync(auth0Id);
            if (existingPlayer == null)
            {
                throw new ArgumentException("Player does not exist.");
            }

            existingPlayer.Name = updatedPlayer.Name ?? existingPlayer.Name;
            existingPlayer.Ranking = updatedPlayer.Ranking ?? existingPlayer.Ranking;
            existingPlayer.Preferred_cue = updatedPlayer.Preferred_cue ?? existingPlayer.Preferred_cue;
            existingPlayer.Profile_picture_url = updatedPlayer.Profile_picture ?? existingPlayer.Profile_picture_url;

            await _playerRepository.UpdatePlayerAsync(existingPlayer);
            return existingPlayer;
        }

        public async Task<Player?> DeletePlayerAsync(int id)
        {
            var existingPlayer = await _playerRepository.GetPlayerByIdAsync(id);
            if (existingPlayer == null)
            {
                throw new ArgumentException("Player does not exist.");
            }

            await _playerRepository.DeletePlayerAsync(existingPlayer);
            return existingPlayer;
        }
    }
}