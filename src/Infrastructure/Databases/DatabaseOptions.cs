using FluentValidation;
using Shared.Options.Abstractions;

namespace Infrastructure.Databases;

public class DatabaseOptions : IConfigurationOptions
{
    public static string SectionName => "Database";

    public string ConnectionString { get; init; } = string.Empty;
}

public class DatabaseOptionsValidator : AbstractValidator<DatabaseOptions>
{
    public DatabaseOptionsValidator()
    {
        RuleFor(o => o.ConnectionString)
            .NotEmpty();
    }
}