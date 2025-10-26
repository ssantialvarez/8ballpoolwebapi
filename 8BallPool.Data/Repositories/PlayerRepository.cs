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

        public async Task AddPlayerAsync(Player player)
        {
            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePlayerAsync(Player player)
        {
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePlayerAsync(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
            }
        }
    }
}