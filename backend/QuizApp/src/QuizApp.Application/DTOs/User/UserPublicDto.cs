using QuizApp.Application.Common.Mappings;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.DTOs.User;

public class UserPublicDto : IMapFrom<Domain.Entities.User>
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string? Photo { get; set; }
}
