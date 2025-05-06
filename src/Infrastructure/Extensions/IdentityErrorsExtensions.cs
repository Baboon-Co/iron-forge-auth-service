using FluentResults;
using Infrastructure.ResultErrors;
using Infrastructure.ResultErrors.Enums;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Extensions;

public static class IdentityErrorsExtensions
{
    private const string DuplicateUserNameCode = nameof(IdentityErrorDescriber.DuplicateUserName);

    private static readonly Dictionary<string, string> CodeToField = new()
    {
        // Username errors
        [nameof(IdentityErrorDescriber.DuplicateUserName)] = "username",
        [nameof(IdentityErrorDescriber.InvalidUserName)] = "username",

        // Email errors
        [nameof(IdentityErrorDescriber.DuplicateEmail)] = "email",
        [nameof(IdentityErrorDescriber.InvalidEmail)] = "email",

        // Password errors
        [nameof(IdentityErrorDescriber.UserAlreadyHasPassword)] = "password",
        [nameof(IdentityErrorDescriber.PasswordMismatch)] = "password",
        [nameof(IdentityErrorDescriber.PasswordTooShort)] = "password",
        [nameof(IdentityErrorDescriber.PasswordRequiresDigit)] = "password",
        [nameof(IdentityErrorDescriber.PasswordRequiresLower)] = "password",
        [nameof(IdentityErrorDescriber.PasswordRequiresUpper)] = "password",
        [nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric)] = "password",
        [nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars)] = "password",

        // Role errors
        [nameof(IdentityErrorDescriber.UserAlreadyInRole)] = "role",
        [nameof(IdentityErrorDescriber.UserNotInRole)] = "role",
        [nameof(IdentityErrorDescriber.InvalidRoleName)] = "role",
        [nameof(IdentityErrorDescriber.DuplicateRoleName)] = "role",

        // Other errors
        [nameof(IdentityErrorDescriber.InvalidToken)] = "token",
        [nameof(IdentityErrorDescriber.ConcurrencyFailure)] = "concurrency",
        [nameof(IdentityErrorDescriber.UserLockoutNotEnabled)] = "lockout"
    };

    private static readonly Dictionary<string, string> CodeToMessage = new()
    {
        // Username messages
        [nameof(IdentityErrorDescriber.DuplicateUserName)] = "User with this username already exists.",
        [nameof(IdentityErrorDescriber.InvalidUserName)] = "Invalid username.",

        // Email messages
        [nameof(IdentityErrorDescriber.DuplicateEmail)] = "User with this email already exists.",
        [nameof(IdentityErrorDescriber.InvalidEmail)] = "Invalid email.",

        // Password messages
        [nameof(IdentityErrorDescriber.UserAlreadyHasPassword)] = "User already has a password.",
        [nameof(IdentityErrorDescriber.PasswordMismatch)] = "Password mismatch.",
        [nameof(IdentityErrorDescriber.PasswordTooShort)] = "Password too short.",
        [nameof(IdentityErrorDescriber.PasswordRequiresDigit)] = "Password requires at least one digit.",
        [nameof(IdentityErrorDescriber.PasswordRequiresLower)] = "Password requires at least one lowercase letter.",
        [nameof(IdentityErrorDescriber.PasswordRequiresUpper)] = "Password requires at least one uppercase letter.",
        [nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric)] =
            "Password requires at least one non-alphanumeric character.",
        [nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars)] =
            "Password requires at least one unique character.",

        // Role messages
        [nameof(IdentityErrorDescriber.UserAlreadyInRole)] = "User already in this role.",
        [nameof(IdentityErrorDescriber.UserNotInRole)] = "User is not in this role.",
        [nameof(IdentityErrorDescriber.InvalidRoleName)] = "Invalid role name.",
        [nameof(IdentityErrorDescriber.DuplicateRoleName)] = "Role with this name already exists.",

        // Other messages
        [nameof(IdentityErrorDescriber.InvalidToken)] = "Invalid token.",
        [nameof(IdentityErrorDescriber.ConcurrencyFailure)] = "Concurrency failure.",
        [nameof(IdentityErrorDescriber.UserLockoutNotEnabled)] = "User lockout is not enabled."
    };

    private static string GetMessage(IdentityError error)
    {
        return CodeToMessage.GetValueOrDefault(
            error.Code,
            !string.IsNullOrWhiteSpace(error.Description)
                ? error.Description
                : "Unknown error.");
    }

    public static Result ToFailedResult(this IdentityResult identityResult)
    {
        if (!identityResult.Succeeded)
            throw new InvalidOperationException("IdentityResult is not succeeded.");

        var errors = new List<Error>
        {
            new GrpcResultError(
                identityResult.Errors.Any(err => err.Code == DuplicateUserNameCode)
                    ? ResponseErrorType.Conflict
                    : ResponseErrorType.InvalidArgument
            )
        };

        var identityErrors = identityResult.Errors
            .Select(err =>
            {
                var field = CodeToField.GetValueOrDefault(err.Code, "unknown");
                var message = GetMessage(err);
                return new FieldError(field, err.Code, message);
            });
        errors.AddRange(identityErrors);

        return Result.Fail(errors);
    }
}