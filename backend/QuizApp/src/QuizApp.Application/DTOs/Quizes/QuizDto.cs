using AutoMapper;
using QuizApp.Application.Common.Mappings;
using QuizApp.Application.DTOs.Questions;
using QuizApp.Application.DTOs.Tags;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Quizes;

public class QuizDto : IMapFrom<Quiz>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public QuizLevel DifficultyLevel { get; set; }
    public int TimeInSeconds { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDeleted { get; set; }

    public int? QuestionCount { get; set; }
    public List<QuestionDto> Questions { get; set; }
    public List<TagDto> Tags { get; set; } = new();

}
