//this file should be the interface for PlayerRepository to implement
using _8BallPool.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _8BallPool.Data.Interfaces
{
    public interface IPlayerRepository
    {
        Task<List<Player>> GetAllPlayersAsync();
        Task<Player?> GetPlayerByIdAsync(int id);
        Task<Player?> GetPlayerByAuth0IdAsync(string auth0Id);
        Task AddPlayerAsync(Player player);
        Task UpdatePlayerAsync(Player player);
        Task DeletePlayerAsync(int id);
    }
}