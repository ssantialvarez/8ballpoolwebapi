using System.Security.Claims;
using _8BallPool.Business.DTOs;
using _8BallPool.Data.Models;
namespace _8BallPool.Business.Interfaces;
public interface IMatchModule
{
    Task<MatchResponseDto?> GetMatchByIdAsync(int id);
    Task<IEnumerable<MatchResponseDto>> GetAllMatchesAsync();
    Task<IEnumerable<MatchResponseDto>> GetMatchesByPlayerIdAsync(int playerId);
    Task<MatchResponseDto?> AddMatchAsync(MatchDto match);
    Task<MatchResponseDto?> UpdateMatchAsync(int id, UpdateMatchDto match);
    Task<MatchResponseDto?> FinishMatchAsync(int matchId, int winnerId, string auth0Id, bool isAdmin = false);
    Task<MatchResponseDto?> DeleteMatchAsync(int matchId, string auth0Id);
}