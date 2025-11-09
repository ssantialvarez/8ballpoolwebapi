// this file should contain methods for accessing and manipulating player data in the database
// using an ORM like Entity Framework Core.
using _8BallPool.Data.Models;
using Microsoft.EntityFrameworkCore;
using _8BallPool.Data.Interfaces;

namespace _8BallPool.Data.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly _8BallPoolContext _context;

        public PlayerRepository(_8BallPoolContext context)
        {
            _context = context;
        }

        public async Task<List<Player>> GetAllPlayersAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _context.Players.FindAsync(id);
        }

        public async Task<Player?> GetPlayerByAuth0IdAsync(string auth0Id)
        {
            return await _context.Players.FirstOrDefaultAsync(p => p.Auth0_id == auth0Id);
        }

        public async Task<Player> AddPlayerAsync(Player player)
        {
            try
            {
                var newPlayer = await _context.Players.AddAsync(player);
                await _context.SaveChangesAsync();
                return newPlayer.Entity;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("23505"))
                {
                    throw new DuplicatePlayerException($"Un jugador con el auth0_id {player.Auth0_id} ya existe.", ex);
                }
                throw;
            }
        }

        public async Task UpdatePlayerAsync(Player player)
        {
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePlayerAsync(Player player)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }
    }
}
