using QuizApp.Application.Common.Mappings;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Questions;

public class QuestionDto : IMapFrom<QuizQuestion>
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public int Points { get; set; }
    public QuestionType Type { get; set; }
    public string Question { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
