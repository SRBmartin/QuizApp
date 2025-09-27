using AutoMapper;
using QuizApp.Application.Common.Mappings;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.DTOs.Tags;

public class TagDto : IMapFrom<Tag>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}
