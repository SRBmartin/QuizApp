using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Infrastructure.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuizApp.Infrastructure.Services.Identity;

public class JwtIdentityService : IIdentityService
{
    private readonly JwtConfiguration _options;
    private readonly SigningCredentials _signing;
    private readonly TokenValidationParameters _validateParams;
    private readonly TokenValidationParameters _ignoreLifetimeParams;

    public JwtIdentityService(IOptions<JwtConfiguration> options)
    {
        _options = options.Value;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
        _signing = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        _validateParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        _ignoreLifetimeParams = _validateParams.Clone();
        _ignoreLifetimeParams.ValidateLifetime = false;
    }

    public async Task<string> GenerateAccessTokenAsync(Guid userId, string username, string role, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, username),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: now.AddMinutes(_options.ExpiryMinutes).UtcDateTime,
            signingCredentials: _signing
        );

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public DateTimeOffset GetAccessTokenExpiryUtc(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        return jwt.ValidTo == DateTime.MinValue ? DateTimeOffset.MinValue : DateTime.SpecifyKind(jwt.ValidTo, DateTimeKind.Utc);
    }

    public ClaimsPrincipal? ValidateAccessToken(string token, bool validateLifetime = true)
    {
        return ValidateInternal(token, validateLifetime ? _validateParams : _ignoreLifetimeParams);
    }

    #region Helpers

    private static ClaimsPrincipal? ValidateInternal(string token, TokenValidationParameters tvp)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            return handler.ValidateToken(token, tvp, out _);
        }
        catch
        {
            return null;
        }
    }

    #endregion

}
