namespace QuizApp.Application.Abstractions.Identity;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? Id { get; }
    string? Username { get; }
    string? Role { get; }
}
