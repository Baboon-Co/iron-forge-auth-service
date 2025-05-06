using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.Options;

public static class OptionsBuilderExtensions
{
    public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(
        this OptionsBuilder<TOptions> builder
    ) where TOptions : class
    {
        builder.Services.AddSingleton<IValidateOptions<TOptions>>(provider =>
            new FluentValidateOptions<TOptions>(provider, builder.Name == string.Empty ? null : builder.Name));

        return builder;
    }
}