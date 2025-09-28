using QuizApp.Application.Common.Mappings;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.DTOs.Questions;

public class QuestionChoiceDto : IMapFrom<QuizQuestionChoice>
{
    public Guid Id { get; set; }
    public string Label { get; set; } = default!;
    public bool IsCorrect { get; set; }
}
