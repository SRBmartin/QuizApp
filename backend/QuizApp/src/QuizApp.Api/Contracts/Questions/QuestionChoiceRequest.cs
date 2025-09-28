namespace QuizApp.Api.Contracts.Questions;

public class QuestionChoiceRequest
{
    public string Label { get; set; } = default!;
    public bool IsCorrect { get; set; }
}
