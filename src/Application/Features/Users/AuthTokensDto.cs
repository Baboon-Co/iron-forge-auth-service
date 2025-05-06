namespace Application.Features.Users;

public record AuthTokensDto(
    string AccessToken,
    DateTime AccessTokenExpiration,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);