
using _8BallPool.Business.DTOs;
using _8BallPool.Business.Interfaces;
using _8BallPool.Data.Interfaces;
using _8BallPool.Data.Models;

namespace _8BallPool.Business.Modules;

public class MatchModule : IMatchModule
{
    private readonly IMatchRepository _matchRepository;
    private readonly IPlayerRepository _playerRepository;
    public MatchModule(IMatchRepository matchRepository, IPlayerRepository playerRepository)
    {
        _matchRepository = matchRepository;
        _playerRepository = playerRepository;
    }

    public async Task<Match?> GetMatchByIdAsync(int id)
    {
        return await _matchRepository.GetMatchByIdAsync(id);
    }

    public async Task<IEnumerable<Match>> GetAllMatchesAsync()
    {
        return await _matchRepository.GetAllMatchesAsync();
    }

    public async Task<Match?> AddMatchAsync(MatchDto match)
    {
        //check if players exist could be added here
        var player1 = await _playerRepository.GetPlayerByIdAsync(match.Player1Id);
        var player2 = await _playerRepository.GetPlayerByIdAsync(match.Player2Id);
        if (player1 == null || player2 == null)
        {
            throw new ArgumentException("One or both players do not exist.");
        }

        var newMatch = new Match
        {
            Player1Id = player1.Id,
            Player2Id = player2.Id,
            StartTime = DateTime.UtcNow
        };

        await _matchRepository.AddMatchAsync(newMatch);
        return newMatch;
    }

    public async Task<Match?> UpdateMatchAsync(int id, MatchDto match)
    {
        var existingMatch = await _matchRepository.GetMatchByIdAsync(id);
        if (existingMatch == null)
        {
            throw new ArgumentException("Match does not exist.");
        }

        var player1 = await _playerRepository.GetPlayerByIdAsync(match.Player1Id);
        var player2 = await _playerRepository.GetPlayerByIdAsync(match.Player2Id);
        if (player1 == null || player2 == null)
        {
            throw new ArgumentException("One or both players do not exist.");
        }

        existingMatch.Player1Id = player1.Id;
        existingMatch.Player2Id = player2.Id;

        await _matchRepository.UpdateMatchAsync(existingMatch);
        return existingMatch;
    }

    public async Task<Match?> DeleteMatchAsync(int matchId)
    {
        var existingMatch = await _matchRepository.GetMatchByIdAsync(matchId);
        if (existingMatch == null)
        {
            throw new ArgumentException("Match does not exist.");
        }
        await _matchRepository.DeleteMatchAsync(existingMatch);
        return existingMatch;
    }
}