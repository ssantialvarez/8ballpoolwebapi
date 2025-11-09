using System.Security.Claims;
using _8BallPool.Business.DTOs;
using _8BallPool.Business.Interfaces;
using _8BallPool.Data.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _8BallPool.WebAPI.Controllers
{
    [ApiController]
    [Route("api/matches")]
    public class MatchController : ControllerBase
    {
        private readonly IMatchModule _matchModule;

        public MatchController(IMatchModule matchModule)
        {
            _matchModule = matchModule;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpGet("", Name = "GetMatches")]
        public async Task<IActionResult> GetMatches()
        {
            try
            {
                var matches = await _matchModule.GetAllMatchesAsync();
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving matches.", details = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpGet("{id}", Name = "GetMatchById")]
        public async Task<IActionResult> GetMatchById(int id)
        {
            try
            {
                var match = await _matchModule.GetMatchByIdAsync(id);
                if (match == null)
                {
                    return NotFound(new { message = "Match not found." });
                }
                return Ok(match);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the match.", details = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpGet("player/{playerId}", Name = "GetMatchesByPlayerId")]
        public async Task<IActionResult> GetMatchesByPlayerId(int playerId)
        {
            try
            {
                var matches = await _matchModule.GetMatchesByPlayerIdAsync(playerId);
                return Ok(matches);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving matches.", details = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("", Name = "CreateMatch")]
        public async Task<IActionResult> CreateMatch([FromBody] MatchDto match)
        {
            try
            {
                var createdMatch = await _matchModule.AddMatchAsync(match);
                return CreatedAtRoute("GetMatchById", new { id = createdMatch!.Id }, createdMatch);
            }
            catch (DoubleBookingException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the match.", details = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "admin")]
        [HttpPut("{id}", Name = "UpdateMatch")]
        public async Task<IActionResult> UpdateMatch(int id, [FromBody] UpdateMatchDto match)
        {
            try
            {
                var updatedMatch = await _matchModule.UpdateMatchAsync(id, match);
                return Ok(updatedMatch);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the match.", details = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpPatch("{id}/finish", Name = "FinishMatch")]
        public async Task<IActionResult> FinishMatch(int id, [FromBody] FinishMatchDto finishMatchDto)
        {
            var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (auth0Id == null)
            {
                return Forbid();
            }

            // Check if user is admin
            var isAdmin = User.IsInRole("admin");

            try
            {
                var finishedMatch = await _matchModule.FinishMatchAsync(id, finishMatchDto.WinnerId, auth0Id, isAdmin);
                return Ok(finishedMatch);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while finishing the match.", details = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpDelete("{id}", Name = "DeleteMatch")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var auth0Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (auth0Id == null)
            {
                return Forbid();
            }

            // Check if user is admin
            var isAdmin = User.IsInRole("admin");

            try
            {
                // If not admin, validate that the user is one of the match participants
                if (!isAdmin)
                {
                    await _matchModule.DeleteMatchAsync(id, auth0Id);
                }
                else
                {
                    // Admin can delete any match without validation
                    var match = await _matchModule.GetMatchByIdAsync(id);
                    if (match == null)
                    {
                        return NotFound(new { message = "Match does not exist." });
                    }
                    await _matchModule.DeleteMatchAsync(id, auth0Id);
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the match.", details = ex.Message });
            }
        }
    }
}
