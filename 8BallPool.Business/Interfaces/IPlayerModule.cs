using System.Security.Claims;
using _8BallPool.Business.DTOs;
using _8BallPool.Data.Models;

namespace _8BallPool.Business.Interfaces
{
    public interface IPlayerModule
    {
        Task<Player> CreatePlayerAsync(PlayerDto player);
        Task<Player> RegisterPlayerAsync(PlayerDto player);
        Task<IEnumerable<Player>> GetPlayersAsync(string? nameFilter = null);
        Task<Player?> GetPlayerByIdAsync(int id);
        Task<Player?> GetPlayerByAuth0IdAsync(ClaimsPrincipal user);
        Task<Player?> UpdatePlayerAsync(int id, UpdatePlayerDto updatedPlayer);
        Task<Player?> UpdatePlayerMeAsync(ClaimsPrincipal user, UpdatePlayerDto updatedPlayer);
        Task<Player?> DeletePlayerAsync(int id);
    }
}