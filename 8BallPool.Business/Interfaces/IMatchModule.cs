using _8BallPool.Business.DTOs;
using _8BallPool.Data.Models;
namespace _8BallPool.Business.Interfaces;
public interface IMatchModule
{
    Task<Match?> GetMatchByIdAsync(int id);
    Task<IEnumerable<Match>> GetAllMatchesAsync();
    Task<Match?> AddMatchAsync(MatchDto match);
    Task<Match?> UpdateMatchAsync(int id, MatchDto match);
    Task<Match?> DeleteMatchAsync(int id);
}