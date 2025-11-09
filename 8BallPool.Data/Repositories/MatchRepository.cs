using _8BallPool.Data.Interfaces;
using _8BallPool.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace _8BallPool.Data.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly _8BallPoolContext _context;
    private const int MatchDurationMinutes = 90;

    public MatchRepository(_8BallPoolContext context)
    {
        _context = context;
    }

    public async Task<Match?> GetMatchByIdAsync(int id)
    {
        return await _context.Matches
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Winner)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Match>> GetAllMatchesAsync()
    {
        return await _context.Matches
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Winner)
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetMatchesByPlayerIdAsync(int playerId)
    {
        return await _context.Matches
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Winner)
            .Where(m => m.Player1Id == playerId || m.Player2Id == playerId)
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingMatchAsync(int playerId, DateTime startTime, int? excludeMatchId = null)
    {
        var endTime = startTime.AddMinutes(MatchDurationMinutes);

        var overlappingMatch = await _context.Matches
            .Where(m => (m.Player1Id == playerId || m.Player2Id == playerId)
                && (excludeMatchId == null || m.Id != excludeMatchId)
                && m.StartTime < endTime
                && (m.EndTime == null || m.EndTime > startTime
                    || m.StartTime.AddMinutes(MatchDurationMinutes) > startTime))
            .AnyAsync();

        return overlappingMatch;
    }

    public async Task<Match> AddMatchAsync(Match match)
    {
        var newMatch = _context.Matches.Add(match);
        await _context.SaveChangesAsync();
        return newMatch.Entity;
    }
    public async Task<Match> UpdateMatchAsync(Match match)
    {
        var updatedMatch = _context.Matches.Update(match);
        await _context.SaveChangesAsync();
        return updatedMatch.Entity;
    }
    public async Task<bool> DeleteMatchAsync(Match match)
    {
        _context.Matches.Remove(match);
        return await _context.SaveChangesAsync() > 0;
    }
}