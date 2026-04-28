using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Photography.Application.Auth;
using Photography.Application.Common.Time;
using Photography.Core.Users;

namespace Photography.Web.Auth;

/// <summary>Strongly-typed JWT options bound from <c>appsettings.json:Jwt</c>.</summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; init; } = "photography-api";
    public string Audience { get; init; } = "photography-client";
    public string Key { get; init; } = string.Empty;
    public int AccessTokenMinutes { get; init; } = 30;
    public int RefreshTokenDays { get; init; } = 14;
}

/// <summary>
/// Issues HS256-signed JWT access tokens (user id, email and role claims) and opaque
/// refresh tokens (32 bytes of cryptographic randomness, base64-url encoded).
/// Refresh tokens are stored as SHA-256 hashes in the user table — the raw value never
/// touches the database.
/// </summary>
public sealed class JwtTokenService(IOptions<JwtOptions> options, IClock clock) : ITokenService
{
    private readonly JwtOptions _opts = options.Value;
    private readonly JwtSecurityTokenHandler _handler = new();

    public (string Token, DateTime ExpiresAtUtc) IssueAccessToken(User user)
    {
        if (string.IsNullOrEmpty(_opts.Key))
            throw new InvalidOperationException("Jwt:Key is not configured");

        var now = clock.UtcNow;
        var expires = now.AddMinutes(_opts.AccessTokenMinutes);
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.Key)),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _opts.Issuer,
            audience: _opts.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            ],
            notBefore: now,
            expires: expires,
            signingCredentials: creds);

        return (_handler.WriteToken(jwt), expires);
    }

    public (string RawToken, string Hash, DateTime ExpiresAtUtc) IssueRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var raw = Base64UrlEncoder.Encode(bytes);
        return (raw, HashRefreshToken(raw), clock.UtcNow.AddDays(_opts.RefreshTokenDays));
    }

    public string HashRefreshToken(string rawToken)
    {
        var bytes = Encoding.UTF8.GetBytes(rawToken);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
