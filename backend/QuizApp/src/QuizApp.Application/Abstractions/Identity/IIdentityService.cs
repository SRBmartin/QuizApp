using System.Security.Claims;

namespace QuizApp.Application.Abstractions.Identity;

public interface IIdentityService
{
    Task<string> GenerateAccessTokenAsync(Guid userId, string username, string role, CancellationToken cancellationToken);
    ClaimsPrincipal? ValidateAccessToken(string token, bool validateLifetime = true);
    DateTimeOffset GetAccessTokenExpiryUtc(string token);
}
