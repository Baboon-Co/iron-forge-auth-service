namespace Application.Features.Users.Login;

public record LoginedUserDto(Guid UserId, AuthTokensDto AuthTokens);