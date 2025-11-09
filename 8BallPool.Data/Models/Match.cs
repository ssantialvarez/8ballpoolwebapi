namespace _8BallPool.Data.Models;

public class Match
{
    public int Id { get; set; }

    // Foreign key for Player 1
    public int Player1Id { get; set; }
    // Required reference navigation to Player 1
    public Player Player1 { get; set; } = null!;

    // Foreign key for Player 2
    public int Player2Id { get; set; }
    // Required reference navigation to Player 2
    public Player Player2 { get; set; } = null!;

    // Required start time
    public DateTime StartTime { get; set; }

    // Optional end time - set when the match finishes
    public DateTime? EndTime { get; set; }

    // Optional foreign key for winner
    public int? WinnerId { get; set; }
    // Optional reference navigation to winner
    public Player? Winner { get; set; }

    // Optional table number
    public int? TableNumber { get; set; }
}