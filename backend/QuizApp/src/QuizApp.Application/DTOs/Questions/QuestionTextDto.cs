using QuizApp.Application.Common.Mappings;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.DTOs.Questions;

public class QuestionTextDto : IMapFrom<QuizQuestionText>
{
    public Guid Id { get; set; }
    public string Text { get; set; } = default!;
}
