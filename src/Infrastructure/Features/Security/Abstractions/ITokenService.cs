using Application.Features.Users;
using Domain;

namespace Infrastructure.Features.Security.Abstractions;

public interface ITokenService
{
    Task<AuthTokensDto> RefreshTokensAsync(User user);
    Task<bool> VerifyRefreshTokenForUserAsync(User user, string refreshToken);
}