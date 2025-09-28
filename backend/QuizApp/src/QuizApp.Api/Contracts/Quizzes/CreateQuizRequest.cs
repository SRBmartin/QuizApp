using QuizApp.Domain.Enums;

namespace QuizApp.Api.Contracts.Quizzes;

public class CreateQuizRequest
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public QuizLevel DifficultyLevel { get; set; }
    public int TimeInSeconds { get; set; }
}
