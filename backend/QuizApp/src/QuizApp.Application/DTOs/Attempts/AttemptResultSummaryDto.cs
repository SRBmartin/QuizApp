using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Attempts;

public class AttemptResultSummaryDto
{
    public Guid AttemptId { get; set; }
    public Guid QuizId { get; set; }
    public string QuizName { get; set; } = string.Empty;

    public AttemptStatus Status { get; set; }

    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }

    public int TotalScore { get; set; }
    public int MaxScore { get; set; }
    public int Percentage { get; set; }

    public int TimeLimitSeconds { get; set; }
    public int? DurationSeconds { get; set; }

    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? SubmittedAt { get; set; }
}