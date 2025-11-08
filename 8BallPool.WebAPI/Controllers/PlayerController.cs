using System.Security.Claims;
using _8BallPool.Business.Interfaces;
using _8BallPool.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _8BallPool.WebAPI.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerModule _playerModule;

        public PlayerController(IPlayerModule playerModule)
        {
            _playerModule = playerModule;
        }
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "admin")]
        [HttpGet("", Name = "GetPlayers")]
        public async Task<IActionResult> GetPlayers()
        {
            var players = await _playerModule.GetPlayersAsync();
            return Ok(players);
        }

        // method POST /players to create a player manually (admin only)
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "admin")]
        [HttpPost("", Name = "CreatePlayer")]
        public async Task<IActionResult> CreatePlayer([FromBody] PlayerDto player)
        {
            var createdPlayer = await _playerModule.CreatePlayerAsync(player);
            return CreatedAtRoute("GetPlayerById", new { id = createdPlayer.Id }, createdPlayer);
        }

        // method POST /players to create a player manually (admin only)
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "app")]
        [HttpPost("/api/auth/register")]
        public async Task<IActionResult> RegisterPlayer([FromBody] PlayerDto player)
        {
            /*
            // use module method to create player
            var clientId = User.FindFirst("sub")?.Value;
            //validate clientId against configuration
            if (clientId != _configuration["ClientId"])
            {
                return Forbid();
            }
            */
            var createdPlayer = await _playerModule.RegisterPlayerAsync(player);
            return CreatedAtRoute("GetPlayerById", new { id = createdPlayer.Id }, createdPlayer);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
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
        [Authorize]
        [HttpGet("me", Name = "GetMyPlayer")]
        public async Task<IActionResult> GetMyPlayer()
        {
            // from token claims, get the Auth0_id of the authenticated user
            var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (auth0Id == null)
            {
                return NotFound();
            }

            var player = await _playerModule.GetPlayerByAuth0IdAsync(auth0Id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpPut("me", Name = "UpdateMyPlayer")]
        public async Task<IActionResult> UpdateMyPlayer([FromBody] Player player)
        {
            // from token claims, get the Auth0_id of the authenticated user
            var auth0Id = User.FindFirst("sub")?.Value;
            if (auth0Id == null)
            {
                return BadRequest();
            }

            var updatedPlayer = await _playerModule.UpdatePlayerMeAsync(auth0Id, player);

            if (updatedPlayer == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}", Name = "DeletePlayer")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            // use module method to delete player by id
            await _playerModule.DeletePlayerAsync(id);
            return NoContent();
        }
    }
}