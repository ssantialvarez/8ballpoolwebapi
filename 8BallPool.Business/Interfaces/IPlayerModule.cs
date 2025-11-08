using _8BallPool.Data.Models;

namespace _8BallPool.Business.Interfaces
{
    public interface IPlayerModule
    {
        Task<Player> CreatePlayerAsync(PlayerDto player);
        Task<Player> RegisterPlayerAsync(PlayerDto player);
        Task<IEnumerable<Player>> GetPlayersAsync(string? nameFilter = null);
        Task<Player?> GetPlayerByIdAsync(int id);
        Task<Player?> GetPlayerByAuth0IdAsync(string auth0Id);
        Task<Player?> UpdatePlayerAsync(int id, Player updatedPlayer);
        Task<bool> DeletePlayerAsync(int id);
    }
}