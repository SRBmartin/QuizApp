namespace QuizApp.Application.DTOs.Quizes;

public class LeaderboardDto
{
    public Guid QuizId { get; set; }
    public string QuizName { get; set; } = default!;
    public string Period { get; set; } = "all";
    public int MaxScore { get; set; }
    public int TotalParticipants { get; set; }

    public List<LeaderboardEntryDto> Top { get; set; } = new();
    public LeaderboardEntryDto? MyEntry { get; set; }
}