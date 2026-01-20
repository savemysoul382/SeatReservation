using FluentValidation.Results;

namespace Shared.Extensions;

public static class ValidationExtensions
{
    public static Failure ToErrors(this ValidationResult validationResult)
    {
        return validationResult.Errors.Select(e => Error.Validation(
            code: e.ErrorCode,
            message: e.ErrorMessage,
            invalidField: e.PropertyName)).ToArray();
    }

    public static Failure ToErrors(this IEnumerable<ValidationResult> validationResults)
    {
        return validationResults.SelectMany(r => r.Errors)
            .Select(e => Error.Validation(
                code: e.ErrorCode,
                message: e.ErrorMessage,
                invalidField: e.PropertyName)).ToArray();
    }
}