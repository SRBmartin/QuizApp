using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Attempts;

public class AttemptReviewItemDto
{
    public Guid QuestionId { get; set; }
    public string Question { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public int Points { get; set; }
    public int AwardedScore { get; set; }
    public bool IsCorrect { get; set; }

    public List<AttemptReviewOptionDto> Options { get; set; } = new();

    public AttemptReviewTextDto? TextAnswer { get; set; }
}