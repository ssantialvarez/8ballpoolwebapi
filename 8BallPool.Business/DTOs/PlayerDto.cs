using _8BallPool.Data.Models;

namespace _8BallPool.Business.DTOs;
public class PlayerDto
{
    public required string Auth0_id { get; set; }
    public required string Name { get; set; }
    public int Ranking { get; set; }
    public string? Preferred_cue { get; set; }
    public required string Profile_picture { get; set; }

    public PlayerDto() { }
    public Player MapToPlayer()
    {
        return new Player
        {
            Auth0_id = this.Auth0_id,
            Name = this.Name,
            Ranking = this.Ranking,
            Preferred_cue = this.Preferred_cue,
            Profile_picture_url = this.Profile_picture
        };
    }
}