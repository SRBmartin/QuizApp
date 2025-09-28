using QuizApp.Application.DTOs.Questions.Emums;

namespace QuizApp.Api.Contracts.Questions;

public class CreateQuestionRequest
{
    public string Question { get; set; } = default!;
    public int Points { get; set; } = default!;
    public QuestionType QuestionType { get; set; }
    public List<QuestionChoiceRequest>? Choices { get; set; }
    public bool? IsTrueCorrect { get; set; }
    public string? TextAnswer { get; set; }
}
