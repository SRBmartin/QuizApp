using QuizApp.Application.DTOs.User;

namespace QuizApp.Application.DTOs.Quizes;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public UserPublicDto User { get; set; } = default!;
    public int TotalScore { get; set; }
    public int Percentage { get; set; }
    public DateTimeOffset? SubmittedAt { get; set; }
}
