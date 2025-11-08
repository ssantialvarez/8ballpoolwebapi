namespace _8BallPool.Business.DTOs;

public class MatchDto
{
    public int Player1Id { get; set; }
    public int Player2Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? WinnerId { get; set; }
    public int? TableNumber { get; set; }

    public MatchDto() { }
}