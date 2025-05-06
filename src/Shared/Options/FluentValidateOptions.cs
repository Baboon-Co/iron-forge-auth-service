using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.Options;

public class FluentValidateOptions<TOptions>(IServiceProvider serviceProvider, string? name)
    : IValidateOptions<TOptions> where TOptions : class
{
    public ValidateOptionsResult Validate(string? optionsName, TOptions options)
    {
        if (name is not null && name != optionsName)
            return ValidateOptionsResult.Skip;

        ArgumentNullException.ThrowIfNull(options);

        using var scope = serviceProvider.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();
        var result = validator.Validate(options);
        if (result.IsValid)
            return ValidateOptionsResult.Success;

        var type = options.GetType().Name;
        var errors = result.Errors
            .Select(failure => $"Validation failed for {type}.{failure.PropertyName} " +
                               $"with the error: {failure.ErrorMessage}");

        return ValidateOptionsResult.Fail(errors);
    }
}