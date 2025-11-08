using _8BallPool.Business.DTOs;
using _8BallPool.Business.Interfaces;
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
            var matches = await _matchModule.GetAllMatchesAsync();
            return Ok(matches);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpGet("{id}", Name = "GetMatchById")]
        public async Task<IActionResult> GetMatchById(int id)
        {
            var match = await _matchModule.GetMatchByIdAsync(id);
            if (match == null)
            {
                return NotFound();
            }
            return Ok(match);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("", Name = "CreateMatch")]
        public async Task<IActionResult> CreateMatch([FromBody] MatchDto match)
        {
            try
            {
                var createdMatch = await _matchModule.AddMatchAsync(match);
                return CreatedAtRoute("GetMatchById", new { id = createdMatch!.Id }, createdMatch);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}", Name = "UpdateMatch")]
        public async Task<IActionResult> UpdateMatch(int id, [FromBody] MatchDto match)
        {
            try
            {
                var updatedMatch = await _matchModule.UpdateMatchAsync(id, match);
                return Ok(updatedMatch);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}", Name = "DeleteMatch")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            try
            {
                await _matchModule.DeleteMatchAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
