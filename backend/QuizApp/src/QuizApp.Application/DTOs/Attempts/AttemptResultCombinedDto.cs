namespace QuizApp.Application.DTOs.Attempts;

public class AttemptResultCombinedDto
{
    public AttemptResultSummaryDto Summary { get; set; } = new();
    public AttemptResultReviewDto Review { get; set; } = new();
}