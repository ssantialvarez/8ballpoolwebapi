using _8BallPool.Data.Models;

namespace _8BallPool.Data.Interfaces
{
    public interface IMatchRepository
    {
        Task<Match?> GetMatchByIdAsync(int id);
        // get matches should accept filters
        /*
        ■​ date=YYYY-MM-DD – returns matches on a specific date.
        ■​ status=upcoming|ongoing|completed – filter by status.
*/
        Task<IEnumerable<Match>> GetAllMatchesAsync();
        Task AddMatchAsync(Match match);
        Task UpdateMatchAsync(Match match);
        Task DeleteMatchAsync(Match match);
    }
}