using _8BallPool.Business.Interfaces;
using _8BallPool.Data.Models;

namespace _8BallPool.Business.Modules
{
    public class PlayerModule : IPlayerModule
    {
        public Task<Player> CreatePlayerAsync(Player player)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Player>> GetPlayersAsync(string? nameFilter = null)
        {
            throw new NotImplementedException();
        }
    }
}