namespace QuizApp.Api.Contracts.Questions;

public class UpdateQuestionRequest
{
    public string Question { get; set; } = default!;
    public int Points { get; set; }
    public List<QuestionChoiceRequest>? Choices { get; set; }
    public bool? IsTrueCorrect { get; set; }
    public string? TextAnswer { get; set; }
}
