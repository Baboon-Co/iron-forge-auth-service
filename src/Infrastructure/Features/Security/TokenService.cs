using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Features.Users;
using Domain;
using Infrastructure.Databases;
using Infrastructure.Features.Security.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Features.Security;

public class TokenService(
    IOptions<JwtOptions> jwtOptions,
    ApplicationDbContext dbContext
) : ITokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthTokensDto> RefreshTokensAsync(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);
        return new AuthTokensDto(
            accessToken,
            DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            refreshToken,
            DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshTokenExpirationMinutes)
        );
    }

    private string GenerateAccessToken(User user)
    {
        var claims = new Claim[] {new(ClaimTypes.NameIdentifier, user.PublicId.ToString())};

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshTokenExpirationMinutes),
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateRefreshTokenAsync(User user)
    {
        var token = Guid.NewGuid().ToString("N");
        var tokenHash = HashRefreshToken(token);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            CreatedAt = DateTime.UtcNow,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshTokenExpirationMinutes),
        };

        var oldToken = await GetCurrentRefreshTokenAsync(user);
        if (oldToken is not null)
            oldToken.ReplacedByToken = refreshToken;
        refreshToken.ReplacesToken = oldToken;

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync();

        return token;
    }

    private async Task<RefreshToken?> GetCurrentRefreshTokenAsync(User user)
    {
        return await dbContext.RefreshTokens
            .Where(rt => !rt.IsRevoked && rt.UserId == user.Id)
            .OrderByDescending(rt => rt.Expiration)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> VerifyRefreshTokenForUserAsync(User user, string incomingToken)
    {
        var currentRefreshToken = await GetCurrentRefreshTokenAsync(user);
        return currentRefreshToken is not null 
               && VerifyRefreshToken(incomingToken, currentRefreshToken.TokenHash);
    }
    
    private bool VerifyRefreshToken(string incomingToken, string storedHash)
    {
        var computedHash = HashRefreshToken(incomingToken);
        var computedBytes = Convert.FromBase64String(computedHash);
        var storedBytes = Convert.FromBase64String(storedHash);
        return CryptographicOperations.FixedTimeEquals(computedBytes, storedBytes);
    }

    private string HashRefreshToken(string token)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = hmac.ComputeHash(tokenBytes);
        return Convert.ToBase64String(hashBytes);
    }
}