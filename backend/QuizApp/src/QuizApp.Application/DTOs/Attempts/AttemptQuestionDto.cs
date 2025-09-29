using QuizApp.Application.DTOs.Questions.Emums;

namespace QuizApp.Application.DTOs.Attempts;

public class AttemptQuestionDto
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public QuestionType Type { get; set; }
    public string Question { get; set; } = default!;
    public int Points { get; set; }
    public List<AttemptQuestionOptionDto> Options { get; set; } = new();
    public bool IsLast { get; set; }
}
