namespace Application.Features.Users.RefreshTokens;

public record RefreshTokensDto(Guid UserPublicId, string RefreshToken);