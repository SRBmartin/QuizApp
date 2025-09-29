using QuizApp.Application.DTOs.User;
using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Attempts;

public class QuizAttemptListItemDto
{
    public Guid AttemptId { get; set; }
    public Guid QuizId { get; set; }
    public string QuizName { get; set; } = default!;

    public UserPublicDto User { get; set; } = default!;

    public AttemptStatus Status { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? SubmittedAt { get; set; }

    public int TotalScore { get; set; }
    public int MaxScore { get; set; }
    public int Percentage { get; set; }

    public int? DurationSeconds { get; set; }
}