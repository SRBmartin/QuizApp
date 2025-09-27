using System.ComponentModel.DataAnnotations;

namespace QuizApp.Infrastructure.Configuration;

public class JwtConfiguration
{
    public const string SectionName = "JwtConfiguration";
    public const string UsernameClaim = "username";
    public const string RoleClaim = "role";

    [Required]
    public string Issuer { get; set; } = null!;
    [Required]
    public string Audience { get; set; } = null!;
    [Required, MinLength(2)]
    public string SecurityKey { get; set; } = null!;
    [Range(1, 1440)]
    public int ExpiryMinutes { get; set; }
}
