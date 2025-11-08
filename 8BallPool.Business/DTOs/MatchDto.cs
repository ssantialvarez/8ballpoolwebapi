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

public class MatchResponseDto : MatchDto
{
    public int Id { get; set; }
    public MatchResponseDto() { }
}