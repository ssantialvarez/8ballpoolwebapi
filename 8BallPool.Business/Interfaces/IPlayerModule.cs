using _8BallPool.Data.Models;

namespace _8BallPool.Business.Interfaces
{
    public interface IPlayerModule
    {
        Task<Player> CreatePlayerAsync(Player player);
        Task<IEnumerable<Player>> GetPlayersAsync(string? nameFilter = null);
    }
}