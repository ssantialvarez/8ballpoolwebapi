namespace _8BallPool.Data.Models;
public class Player
{
    public int Id { get; set; }
    public required string Auth0_id { get; set; }
    public required string Name { get; set; }
    public int Ranking { get; set; }
    public string? Preferred_cue { get; set; }
    public required string Profile_picture_url { get; set; }
}