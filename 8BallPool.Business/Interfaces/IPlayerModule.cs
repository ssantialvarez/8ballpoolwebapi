using _8BallPool.Data.Models;

namespace _8BallPool.Business.Interfaces
{
    public interface IPlayerModule
    {
        Task<Player> CreatePlayerAsync(Player player);
        Task<IEnumerable<Player>> GetPlayersAsync(string? nameFilter = null);
        Task<Player> GetPlayerByIdAsync(int id);
        Task<Player?> UpdatePlayerAsync(int id, Player updatedPlayer);
        Task<bool> DeletePlayerAsync(int id);
    }
}