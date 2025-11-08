using _8BallPool.Data.Interfaces;
using _8BallPool.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace _8BallPool.Data.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly _8BallPoolContext _context;

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
    public async Task AddMatchAsync(Match match)
    {
        _context.Matches.Add(match);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateMatchAsync(Match match)
    {
        _context.Matches.Update(match);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteMatchAsync(Match match)
    {
        _context.Matches.Remove(match);
        await _context.SaveChangesAsync();
    }
}