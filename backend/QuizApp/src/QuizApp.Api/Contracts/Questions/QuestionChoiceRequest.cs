namespace QuizApp.Api.Contracts.Questions;

public class QuestionChoiceRequest
{
    public Guid? Id { get; set; } = default!;
    public string Label { get; set; } = default!;
    public bool IsCorrect { get; set; }
}
