namespace QuizApp.Api.Contracts.Attempts;

public class SaveAnswerRequest
{
    public List<Guid>? SelectedChoiceIds { get; set; }
    public string? SubmittedText { get; set; }
}
