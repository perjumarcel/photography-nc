namespace Photography.Application.Auth.Dtos;

public sealed record LoginRequestDto(string Email, string Password);

public sealed record RefreshRequestDto(string RefreshToken);

public sealed record TokensDto(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);
