using QuizApp.Application.Common.Mappings;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Attempts;

public class AttemptDto : IMapFrom<Attempt>
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public AttemptStatus Status { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? SubmittedAt { get; set; }
    public int TotalScore { get; set; }
    public int TimeLimitSeconds { get; set; }
    public int AnsweredCount { get; set; }
    public int TotalQuestions { get; set; }
    public int TimeLeftSeconds { get; set; }
}
