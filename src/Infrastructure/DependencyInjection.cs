using Application.Features.Users.Abstractions;
using Domain;
using FluentValidation;
using Infrastructure.Databases;
using Infrastructure.Features.Security;
using Infrastructure.Features.Security.Abstractions;
using Infrastructure.Features.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Utility.Configuration.Options;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddDatabase();
        services.AddJwt();

        // Services
        services.AddScoped<IUserService, UserService>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddOptionsWithFluentValidation<DatabaseOptions>();
        services.AddDbContext<ApplicationDbContext>((provider, options) =>
        {
            var dbOptions = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(dbOptions.ConnectionString);
        });

        return services;
    }

    private static IServiceCollection AddJwt(this IServiceCollection services)
    {
        services.AddOptionsWithFluentValidation<JwtOptions>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}