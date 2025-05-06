using Application.Features.Users.Login;
using Application.Features.Users.RefreshTokens;
using Application.Features.Users.Register;
using FluentResults;

namespace Application.Features.Users.Abstractions;

public interface IUserService
{
    Task<Result<RegisteredUserDto>> RegisterAsync(RegisterDto dto);
    Task<Result<LoginedUserDto>> Login(LoginDto dto);
    Task<Result<AuthTokensDto>> RefreshTokens(RefreshTokensDto dto);
}