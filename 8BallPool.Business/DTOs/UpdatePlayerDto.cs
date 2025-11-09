using _8BallPool.Data.Models;

namespace _8BallPool.Business.DTOs;
public class UpdatePlayerDto
{
    public string? Name { get; set; }
    public int? Ranking { get; set; }
    public string? Preferred_cue { get; set; }
    public string? Profile_picture { get; set; }
    
    public UpdatePlayerDto() { }
    public Player MapToPlayer(string auth0Id)
    {
        return new Player
        {
            Auth0_id = auth0Id,
            Name = this.Name ?? string.Empty,
            Ranking = this.Ranking ?? 0,
            Preferred_cue = this.Preferred_cue ?? string.Empty,
            Profile_picture_url = this.Profile_picture ?? string.Empty
        };
    }
}