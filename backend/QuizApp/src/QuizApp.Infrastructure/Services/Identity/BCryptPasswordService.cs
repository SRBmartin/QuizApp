using QuizApp.Application.Abstractions.Identity;

namespace QuizApp.Infrastructure.Services.Identity;

public class BCryptPasswordService : IPasswordService
{
    private const int WorkFactor = 12;

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
