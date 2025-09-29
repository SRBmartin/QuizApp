namespace QuizApp.Application.DTOs.Attempts;

public class StartAttemptDto
{
    public Guid AttemptId { get; set; }
    public int TimeLeftSeconds { get; set; }
    public int TimeLimitSeconds { get; set; }
    public AttemptQuestionDto Question { get; set; } = default!;
}
