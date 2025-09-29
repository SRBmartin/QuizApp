namespace QuizApp.Application.DTOs.Attempts;

public class AttemptResultReviewDto
{
    public Guid AttemptId { get; set; }
    public List<AttemptReviewItemDto> Items { get; set; } = new();
}