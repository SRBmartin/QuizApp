namespace QuizApp.Application.DTOs.Attempts;

public class AttemptReviewOptionDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool? IsCorrect { get; set; }
    public bool? SelectedByUser { get; set; }
}