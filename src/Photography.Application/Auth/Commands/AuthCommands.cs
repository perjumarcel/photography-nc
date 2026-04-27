using MediatR;
using Photography.Application.Auth.Dtos;
using Photography.Application.Common.Time;
using Photography.Core.Users;
using Photography.SharedKernel;

namespace Photography.Application.Auth.Commands;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<TokensDto>>;

/// <summary>
/// Verifies a user's email and password and issues a fresh access+refresh token pair.
/// To prevent user-enumeration via timing, the hasher is invoked on a dummy hash when
/// no user is found, so successful and failed lookups take comparable time.
/// </summary>
public sealed class LoginHandler(
    IUserRepository users,
    IPasswordHasher hasher,
    ITokenService tokens) : IRequestHandler<LoginCommand, Result<TokensDto>>
{
    // A pre-computed PBKDF2 hash of an unguessable random secret. Used only to keep
    // verification timing consistent when the user does not exist.
    private const string DummyHash = "v1.100000.AAAAAAAAAAAAAAAAAAAAAA==.AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

    public async Task<Result<TokensDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Result<TokensDto>.Unauthorized("Invalid credentials");

        var user = await users.GetByEmailAsync(request.Email, ct);
        var verified = hasher.Verify(user?.PasswordHash ?? DummyHash, request.Password);
        if (user is null || !verified)
            return Result<TokensDto>.Unauthorized("Invalid credentials");

        var (access, accessExp) = tokens.IssueAccessToken(user);
        var (raw, hash, refreshExp) = tokens.IssueRefreshToken();
        user.IssueRefreshToken(hash, refreshExp);
        await users.SaveChangesAsync(ct);

        return Result<TokensDto>.Ok(new TokensDto(access, accessExp, raw, refreshExp));
    }
}

public sealed record RefreshCommand(string RefreshToken) : IRequest<Result<TokensDto>>;

/// <summary>
/// Rotates the refresh token. The presented raw token is hashed and used as a lookup
/// key into the user store; if the lookup hits and the stored token has not expired,
/// a fresh access+refresh pair is issued and the previous refresh token is invalidated
/// (single-use rotation).
/// </summary>
public sealed class RefreshHandler(
    IUserRepository users,
    ITokenService tokens,
    IClock clock) : IRequestHandler<RefreshCommand, Result<TokensDto>>
{
    public async Task<Result<TokensDto>> Handle(RefreshCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Result<TokensDto>.Unauthorized("Invalid refresh token");

        var candidateHash = tokens.HashRefreshToken(request.RefreshToken);
        var user = await users.GetByRefreshTokenHashAsync(candidateHash, ct);
        if (user is null || !user.RefreshTokenMatches(candidateHash, clock.UtcNow))
            return Result<TokensDto>.Unauthorized("Invalid refresh token");

        var (access, accessExp) = tokens.IssueAccessToken(user);
        var (raw, hash, refreshExp) = tokens.IssueRefreshToken();
        user.IssueRefreshToken(hash, refreshExp);
        await users.SaveChangesAsync(ct);

        return Result<TokensDto>.Ok(new TokensDto(access, accessExp, raw, refreshExp));
    }
}

public sealed record LogoutCommand(Guid UserId) : IRequest<Result>;

/// <summary>Idempotent logout — clears any active refresh token for the user.</summary>
public sealed class LogoutHandler(IUserRepository users) : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(request.UserId, ct);
        if (user is null) return Result.Ok();
        user.RevokeRefreshToken();
        await users.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
