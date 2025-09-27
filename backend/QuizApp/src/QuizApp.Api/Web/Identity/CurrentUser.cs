using QuizApp.Application.Abstractions.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace QuizApp.Api.Web.Identity;

public class CurrentUser(
    IHttpContextAccessor httpContextAccessor
) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public Guid? Id
    {
        get
        {
            var sub = Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? Username => Principal?.FindFirst("username")?.Value;

    public string? Role => Principal?.FindFirst("role")?.Value
    ?? Principal?.FindFirst(ClaimTypes.Role)?.Value
    ?? Principal?.Claims.FirstOrDefault(c =>
           c.Type.Equals("roles", StringComparison.OrdinalIgnoreCase) ||
           c.Type.EndsWith("/role", StringComparison.OrdinalIgnoreCase) ||
           c.Type.EndsWith("/roles", StringComparison.OrdinalIgnoreCase))?.Value;

}
