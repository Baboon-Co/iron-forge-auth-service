using Application.Features.Users;
using Application.Features.Users.Abstractions;
using Application.Features.Users.Login;
using Application.Features.Users.RefreshTokens;
using Application.Features.Users.Register;
using Domain;
using FluentResults;
using Infrastructure.Extensions;
using Infrastructure.Features.Security.Abstractions;
using Infrastructure.ResultErrors;
using Infrastructure.ResultErrors.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Features.Users;

public class UserService(
    UserManager<User> userManager,
    ITokenService tokenService,
    ILogger<UserService> logger
) : IUserService
{
    public async Task<Result<RegisteredUserDto>> RegisterAsync(RegisterDto dto)
    {
        var user = new User
        {
            PublicId = Guid.NewGuid(),
            UserName = dto.Login
        };

        var creation = await userManager.CreateAsync(user, dto.Password);

        if (creation.Succeeded)
        {
            var tokens = await tokenService.RefreshTokensAsync(user);
            return Result.Ok(new RegisteredUserDto(user.PublicId, tokens));
        }

        var creationFailedResult = creation.ToFailedResult();
        logger.LogDebug("{Errors}", creationFailedResult.Errors);

        return creationFailedResult;
    }

    public async Task<Result<LoginedUserDto>> Login(LoginDto dto)
    {
        var user = await userManager.FindByNameAsync(dto.Login);
        if (user == null || !await userManager.CheckPasswordAsync(user, dto.Password))
        {
            return Result.Fail(new GrpcResultError(
                ResponseErrorType.Unauthenticated,
                "Login failed: Invalid login or password.")
            );
        }

        var tokens = await tokenService.RefreshTokensAsync(user);
        return Result.Ok(new LoginedUserDto(user.PublicId, tokens));
    }

    public async Task<Result<AuthTokensDto>> RefreshTokens(RefreshTokensDto dto)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.PublicId == dto.UserPublicId);
        if (user == null || !await tokenService.VerifyRefreshTokenForUserAsync(user, dto.RefreshToken))
        {
            return Result.Fail(new GrpcResultError(
                ResponseErrorType.Unauthorized,
                "Token refresh failed: Invalid refresh token for user.")
            );
        }

        var tokens = await tokenService.RefreshTokensAsync(user);
        return Result.Ok(tokens);
    }
}