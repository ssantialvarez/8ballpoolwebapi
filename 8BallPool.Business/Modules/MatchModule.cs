
using System.Security.Claims;
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

    public async Task<MatchResponseDto?> GetMatchByIdAsync(int id)
    {
        var match = await _matchRepository.GetMatchByIdAsync(id);
        if (match == null) return null;

        return new MatchResponseDto
        {
            Id = match.Id,
            Player1Id = match.Player1Id,
            Player2Id = match.Player2Id,
            StartTime = match.StartTime,
            EndTime = match.EndTime,
            WinnerId = match.WinnerId,
            TableNumber = match.TableNumber
        };
    }

    public async Task<IEnumerable<MatchResponseDto>> GetAllMatchesAsync()
    {
        var matches = await _matchRepository.GetAllMatchesAsync();
        return matches.Select(match => new MatchResponseDto
        {
            Id = match.Id,
            Player1Id = match.Player1Id,
            Player2Id = match.Player2Id,
            StartTime = match.StartTime,
            EndTime = match.EndTime,
            WinnerId = match.WinnerId,
            TableNumber = match.TableNumber
        });
    }

    public async Task<IEnumerable<MatchResponseDto>> GetMatchesByPlayerIdAsync(int playerId)
    {
        // Validate that the player exists
        var player = await _playerRepository.GetPlayerByIdAsync(playerId);
        if (player == null)
        {
            throw new ArgumentException("Player does not exist.");
        }

        var matches = await _matchRepository.GetMatchesByPlayerIdAsync(playerId);
        return matches.Select(match => new MatchResponseDto
        {
            Id = match.Id,
            Player1Id = match.Player1Id,
            Player2Id = match.Player2Id,
            StartTime = match.StartTime,
            EndTime = match.EndTime,
            WinnerId = match.WinnerId,
            TableNumber = match.TableNumber
        });
    }

    public async Task<MatchResponseDto?> AddMatchAsync(MatchDto match)
    {
        try
        {
            var player1 = await _playerRepository.GetPlayerByIdAsync(match.Player1Id);
            var player2 = await _playerRepository.GetPlayerByIdAsync(match.Player2Id);
            if (player1 is null || player2 is null)
            {
                return null;
            }

            var newMatch = new Match
            {
                Player1Id = player1.Id,
                Player2Id = player2.Id,
                StartTime = match.StartTime,
            };

            newMatch = await _matchRepository.AddMatchAsync(newMatch);

            return new MatchResponseDto
            {
                Id = newMatch.Id,
                Player1Id = newMatch.Player1Id,
                Player2Id = newMatch.Player2Id,
                StartTime = newMatch.StartTime,
                EndTime = newMatch.EndTime,
                WinnerId = newMatch.WinnerId,
                TableNumber = newMatch.TableNumber
            };
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while adding the match.", ex);
        }
    }

    public async Task<MatchResponseDto?> UpdateMatchAsync(int id, UpdateMatchDto match)
    {
        var existingMatch = await _matchRepository.GetMatchByIdAsync(id);
        if (existingMatch == null)
        {
            throw new ArgumentException("Match does not exist.");
        }

        // Only allow updating TableNumber
        if (match.TableNumber.HasValue)
        {
            existingMatch.TableNumber = match.TableNumber.Value;
        }

        var updatedMatch = await _matchRepository.UpdateMatchAsync(existingMatch);
        return updatedMatch != null ? new MatchResponseDto
        {
            Id = updatedMatch.Id,
            Player1Id = updatedMatch.Player1Id,
            Player2Id = updatedMatch.Player2Id,
            StartTime = updatedMatch.StartTime,
            EndTime = updatedMatch.EndTime,
            WinnerId = updatedMatch.WinnerId,
            TableNumber = updatedMatch.TableNumber
        } : null;
    }

    public async Task<MatchResponseDto?> FinishMatchAsync(int matchId, int winnerId, string auth0Id, bool isAdmin = false)
    {
        var existingMatch = await _matchRepository.GetMatchByIdAsync(matchId);
        if (existingMatch == null)
        {
            throw new ArgumentException("Match does not exist.");
        }

        // Validate that the match is not already finished
        if (existingMatch.EndTime != null)
        {
            throw new InvalidOperationException("Match is already finished.");
        }

        // Validate that the winner is one of the players in the match
        if (winnerId != existingMatch.Player1Id && winnerId != existingMatch.Player2Id)
        {
            throw new ArgumentException("Winner must be one of the players in the match.");
        }

        // Only validate participant authorization if not admin
        if (!isAdmin)
        {
            // Get the player by auth0Id to verify authorization
            var player = await _playerRepository.GetPlayerByAuth0IdAsync(auth0Id);
            if (player == null)
            {
                throw new UnauthorizedAccessException("Player not found.");
            }

            // Check if the player is one of the participants in the match
            if (existingMatch.Player1Id != player.Id && existingMatch.Player2Id != player.Id)
            {
                throw new UnauthorizedAccessException("You are not authorized to finish this match.");
            }
        }

        existingMatch.EndTime = DateTime.UtcNow;
        existingMatch.WinnerId = winnerId;

        await _matchRepository.UpdateMatchAsync(existingMatch);
        return existingMatch != null ? new MatchResponseDto
        {
            Id = existingMatch.Id,
            Player1Id = existingMatch.Player1Id,
            Player2Id = existingMatch.Player2Id,
            StartTime = existingMatch.StartTime,
            EndTime = existingMatch.EndTime,
            WinnerId = existingMatch.WinnerId,
            TableNumber = existingMatch.TableNumber
        } : null;
    }

    public async Task<MatchResponseDto?> DeleteMatchAsync(int matchId, string auth0Id)
    {
        var existingMatch = await _matchRepository.GetMatchByIdAsync(matchId);
        if (existingMatch == null)
        {
            throw new ArgumentException("Match does not exist.");
        }

        // Get the player by auth0Id
        var player = await _playerRepository.GetPlayerByAuth0IdAsync(auth0Id);
        if (player == null)
        {
            throw new UnauthorizedAccessException("Player not found.");
        }

        // Check if the player is one of the participants in the match
        if (existingMatch.Player1Id != player.Id && existingMatch.Player2Id != player.Id)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this match.");
        }

        await _matchRepository.DeleteMatchAsync(existingMatch);
        return  existingMatch != null ? new MatchResponseDto
        {
            Id = existingMatch.Id,
            Player1Id = existingMatch.Player1Id,
            Player2Id = existingMatch.Player2Id,
            StartTime = existingMatch.StartTime,
            EndTime = existingMatch.EndTime,
            WinnerId = existingMatch.WinnerId,
            TableNumber = existingMatch.TableNumber
        } : null;
    }
}