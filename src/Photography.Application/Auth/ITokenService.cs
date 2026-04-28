using Photography.Core.Users;

namespace Photography.Application.Auth;

/// <summary>
/// Issues short-lived JWT access tokens and opaque refresh tokens.
/// Implementation lives in the Web layer where the signing key is bound to configuration.
/// </summary>
public interface ITokenService
{
    /// <summary>Issue a JWT for the given user. Returns the JWT and its expiry timestamp (UTC).</summary>
    (string Token, DateTime ExpiresAtUtc) IssueAccessToken(User user);

    /// <summary>Generate a fresh opaque refresh token along with its hash and expiry.</summary>
    (string RawToken, string Hash, DateTime ExpiresAtUtc) IssueRefreshToken();

    /// <summary>Compute the deterministic hash of a candidate refresh token (matches <see cref="IssueRefreshToken"/>).</summary>
    string HashRefreshToken(string rawToken);
}
