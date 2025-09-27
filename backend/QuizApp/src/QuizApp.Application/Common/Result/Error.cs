using System.Security.Cryptography.X509Certificates;

namespace QuizApp.Application.Common.Result;

public record struct Error
(
    string Code,
    string Message
)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}