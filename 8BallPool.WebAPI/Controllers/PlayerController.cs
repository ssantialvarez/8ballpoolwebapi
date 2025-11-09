using System.Security.Claims;
using _8BallPool.Business.DTOs;
using _8BallPool.Business.Interfaces;
using _8BallPool.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace _8BallPool.WebAPI.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerModule _playerModule;
        private readonly IMatchModule _matchModule;

        public PlayerController(IPlayerModule playerModule, IMatchModule matchModule)
        {
            _playerModule = playerModule;
            _matchModule = matchModule;
        }
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "admin")]
        [HttpGet("", Name = "GetPlayers")]
        public async Task<IActionResult> GetPlayers([FromQuery] string? name = null)
        {
            try
            {
                var players = await _playerModule.GetPlayersAsync(name);
                return Ok(players);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving players.", details = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "admin")]
        [HttpPost("", Name = "CreatePlayer")]
        public async Task<IActionResult> CreatePlayer([FromBody] PlayerDto player)
        {
            try
            {
                var createdPlayer = await _playerModule.CreatePlayerAsync(player);
                return CreatedAtRoute("GetPlayerById", new { id = createdPlayer.Id }, createdPlayer);
            }
            catch (DuplicatePlayerException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the player.", details = ex.Message });
            }
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
            try
            {
                var createdPlayer = await _playerModule.RegisterPlayerAsync(player);
                return CreatedAtRoute("GetPlayerById", new { id = createdPlayer.Id }, createdPlayer);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while registering the player.", details = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpGet("{id}", Name = "GetPlayerById")]
        public async Task<IActionResult> GetPlayerById(int id)
        {
            try
            {
                var player = await _playerModule.GetPlayerByIdAsync(id);
                if (player == null)
                {
                    return NotFound(new { message = "Player not found." });
                }
                return Ok(player);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the player.", details = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpGet("me", Name = "GetMyPlayer")]
        public async Task<IActionResult> GetMyPlayer()
        {
            try
            {
                var player = await _playerModule.GetPlayerByAuth0IdAsync(User);
                if (player == null)
                {
                    return NotFound(new { message = "Player not found." });
                }
                return Ok(player);    
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving your player profile.", details = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpPut("me", Name = "UpdateMyPlayer")]
        public async Task<IActionResult> UpdateMyPlayer([FromBody] UpdatePlayerDto player)
        {
            try
            {
                var updatedPlayer = await _playerModule.UpdatePlayerMeAsync(User, player);
                return Ok(updatedPlayer);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating your player profile.", details = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}", Name = "DeletePlayer")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            try
            {
                var deletedPlayer = await _playerModule.DeletePlayerAsync(id);
                return Ok(deletedPlayer);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the player.", details = ex.Message });
            }
        }
    }
}