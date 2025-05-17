using Application.Features.Users.Abstractions;
using Application.Features.Users.Login;
using Application.Features.Users.RefreshTokens;
using Application.Features.Users.Register;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IronForge.Contracts.AuthService;
using Utility.Grpc;

namespace Api.Controllers.GrpcControllers;

public class AuthGrpcController(
    IUserService userService,
    ILogger<AuthGrpcController> logger
) : Auth.AuthBase
{
    public override async Task<RegisterResponse> Register(
        RegisterRequest request,
        ServerCallContext context)
    {
        var registerDto = new RegisterDto(request.Login, request.Password);
        var registerResult = await userService.RegisterAsync(registerDto);

        if (registerResult.IsSuccess)
        {
            logger.LogInformation("User \"{Username}\" registered.", request.Login);

            var userId = registerResult.Value.UserId.ToString();
            var authTokens = registerResult.Value.AuthTokens;
            return new RegisterResponse
            {
                UserId = userId,
                AccessToken = authTokens.AccessToken,
                AccessTokenExpiration = Timestamp.FromDateTime(authTokens.AccessTokenExpiration),
                RefreshToken = authTokens.RefreshToken,
                RefreshTokenExpiration = Timestamp.FromDateTime(authTokens.RefreshTokenExpiration)
            };
        }

        throw GrpcHelper.CreateRpcException(registerResult);
    }

    public override async Task<LoginResponse> Login(
        LoginRequest request,
        ServerCallContext context)
    {
        var loginDto = new LoginDto(request.Login, request.Password);
        var loginResult = await userService.Login(loginDto);
        if (loginResult.IsSuccess)
        {
            logger.LogInformation("User \"{Username}\" logged in.", request.Login);

            var userId = loginResult.Value.UserId.ToString();
            var authTokens = loginResult.Value.AuthTokens;
            return new LoginResponse
            {
                UserId = userId,
                AccessToken = authTokens.AccessToken,
                AccessTokenExpiration = Timestamp.FromDateTime(authTokens.AccessTokenExpiration),
                RefreshToken = authTokens.RefreshToken,
                RefreshTokenExpiration = Timestamp.FromDateTime(authTokens.RefreshTokenExpiration)
            };
        }

        throw GrpcHelper.CreateRpcException(loginResult);
    }

    public override async Task<RefreshTokensResponse> RefreshTokens(
        RefreshTokensRequest request,
        ServerCallContext context)
    {
        var refreshTokensDto = new RefreshTokensDto(Guid.Parse(request.UserId), request.RefreshToken);
        var refreshTokensResult = await userService.RefreshTokens(refreshTokensDto);

        var authTokens = refreshTokensResult.Value;
        var response = new RefreshTokensResponse
        {
            AccessToken = authTokens.AccessToken,
            AccessTokenExpiration = Timestamp.FromDateTime(authTokens.AccessTokenExpiration),
            RefreshToken = authTokens.RefreshToken,
            RefreshTokenExpiration = Timestamp.FromDateTime(authTokens.RefreshTokenExpiration)
        };

        logger.LogDebug("User with public id: {PublicId} refreshed tokens.", request.UserId);

        return response;
    }
}