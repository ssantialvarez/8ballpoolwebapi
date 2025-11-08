using _8BallPool.Data.Models;

namespace _8BallPool.Data.Interfaces
{
    public interface IMatchRepository
    {
        Task<Match?> GetMatchByIdAsync(int id);
        Task<IEnumerable<Match>> GetAllMatchesAsync();
        Task<IEnumerable<Match>> GetMatchesByPlayerIdAsync(int playerId);
        Task<Match?> AddMatchAsync(Match match);
        Task<Match?> UpdateMatchAsync(Match match);
        Task<bool> DeleteMatchAsync(Match match);
    }
}