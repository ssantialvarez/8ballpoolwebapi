using _8BallPool.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace _8BallPool.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerModule _playerModule;
        public PlayerController(IPlayerModule playerModule)
        {
            _playerModule = playerModule;
        }
        public async Task<IActionResult> GetPlayers()
        {
            // Implementation for fetching players will go here
            return Ok();
        }
    }
}

/*
●​ POST /auth/register – Registers a new user (handled via Auth0 callback,
auto-creates a player).
●​ POST /players – Create a player manually (admin only).
●​ GET /players – List all players (admin only). Allow optional filtering by
name.
●​ GET /players/me – Fetch the authenticated user's player details.
●​ PATCH/PUT /players/me – Update the authenticated user's player details.
●​ DELETE /players/:id – Delete a player (admin only).
*/