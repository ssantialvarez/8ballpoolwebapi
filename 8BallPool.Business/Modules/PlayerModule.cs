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
            var newPlayer = new Player
            {
                Auth0_id = player.Auth0_id,
                Name = player.Name,
                Ranking = player.Ranking,
                Preferred_cue = player.Preferred_cue,
                Profile_picture_url = player.Profile_picture_url
            };

            await _playerRepository.AddPlayerAsync(newPlayer);
            return newPlayer;
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync(string? nameFilter = null)
        {
            var players = await _playerRepository.GetAllPlayersAsync();
            if (!string.IsNullOrEmpty(nameFilter))
            {
                players = players.Where(p => p.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return players;
        }

        public async Task<Player> RegisterPlayerAsync(PlayerDto player)
        {
            //first check if player with Auth0_id already exists
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
                Profile_picture_url = player.Profile_picture_url
            };

            await _playerRepository.AddPlayerAsync(newPlayer);
            return newPlayer;
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _playerRepository.GetPlayerByIdAsync(id);
        }

        public async Task<Player?> GetPlayerByAuth0IdAsync(string auth0Id)
        {
            return await _playerRepository.GetPlayerByAuth0IdAsync(auth0Id);
        }

        public async Task<Player?> UpdatePlayerAsync(int id, Player updatedPlayer)
        {
            var existingPlayer = await _playerRepository.GetPlayerByIdAsync(id);
            if (existingPlayer == null)
            {
                return null;
            }

            existingPlayer.Name = updatedPlayer.Name;
            existingPlayer.Ranking = updatedPlayer.Ranking;

            await _playerRepository.UpdatePlayerAsync(existingPlayer);
            return existingPlayer;
        }

        public async Task<bool> DeletePlayerAsync(int id)
        {
            var existingPlayer = await _playerRepository.GetPlayerByIdAsync(id);
            if (existingPlayer == null)
            {
                return false;
            }

            await _playerRepository.DeletePlayerAsync(id);
            return true;
        }
    }
}