namespace QuizApp.Application.DTOs.Quizes;

public class GlobalLeaderboardDto
{
    public string Period { get; set; } = "all";
    public int MaxScoreTotal { get; set; }
    public int TotalParticipants { get; set; }
    public List<LeaderboardEntryDto> Top { get; set; } = new();
    public LeaderboardEntryDto? MyEntry { get; set; }
}