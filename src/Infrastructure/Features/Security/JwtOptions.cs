using BaboonCo.Utility.Configuration.Options.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Features.Security;

public class JwtOptions : IConfigurationOptions
{
    public static string SectionName => "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationMinutes { get; set; }
}

public class JwtOptionsValidator : AbstractValidator<JwtOptions>
{
    public JwtOptionsValidator(IHostEnvironment environment)
    {
        RuleFor(o => o.Secret)
            .NotEqual("DEV_ONLY_KEY_DO_NOT_USE_IN_PRODUCTION").When(_ => environment.IsProduction())
            .Length(32, 64)
            .NotEmpty();

        RuleFor(o => o.Issuer)
            .NotEmpty();

        RuleFor(o => o.Audience)
            .NotEmpty();

        RuleFor(o => o.AccessTokenExpirationMinutes)
            .GreaterThan(0)
            .LessThan(1440);

        RuleFor(o => o.RefreshTokenExpirationMinutes)
            .GreaterThan(0)
            .LessThan(1440);
    }
}