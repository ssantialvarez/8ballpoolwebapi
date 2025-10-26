using _8BallPool.Business.Interfaces;
using _8BallPool.Data.Models;
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
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("", Name = "GetPlayers")]
        public async Task<IActionResult> GetPlayers()
        {
            // Implementation for fetching players will go here
            // use module method to get all players
            var players = await _playerModule.GetPlayersAsync();
            return Ok(players);
        }

        // method POST /players to create a player manually (admin only)
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("", Name = "CreatePlayer")]
        public async Task<IActionResult> CreatePlayer([FromBody] Player player)
        {
            // use module method to create player
            var createdPlayer = await _playerModule.CreatePlayerAsync(player);
            return CreatedAtRoute("GetPlayerById", new { id = createdPlayer.Id }, createdPlayer);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}", Name = "GetPlayerById")]
        public async Task<IActionResult> GetPlayerById(int id)
        {
            // use module method to get player by id
            var player = await _playerModule.GetPlayerByIdAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("me", Name = "GetMyPlayer")]
        public async Task<IActionResult> GetMyPlayer()
        {
            // Implementation for fetching the authenticated user's player details will go here
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("me", Name = "UpdateMyPlayer")]
        public async Task<IActionResult> UpdateMyPlayer()
        {
            // Implementation for updating the authenticated user's player details will go here
            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}", Name = "DeletePlayer")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            // use module method to delete player by id
            await _playerModule.DeletePlayerAsync(id);
            return NoContent();
        }
    }
}