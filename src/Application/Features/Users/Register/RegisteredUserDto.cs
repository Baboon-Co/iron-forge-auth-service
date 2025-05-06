namespace Application.Features.Users.Register;

public record RegisteredUserDto(Guid UserId, AuthTokensDto AuthTokens);