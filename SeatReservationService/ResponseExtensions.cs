// Shared

using Microsoft.AspNetCore.Mvc;
using Shared;

namespace SeatReservationService;

public static class ResponseExtensions
{
    public static IActionResult ToResponse(this Failure failure)
    {
        if (!failure.Any())
        {
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }

        var distinctErrorTypes = failure
            .Select(e => e.Type)
            .Distinct()
            .ToList();

        int statusCode = distinctErrorTypes.Count > 1
            ? StatusCodes.Status500InternalServerError
            : GetStatusCodeFromErrorType(distinctErrorTypes.First());

        return new ObjectResult(value: failure)
        {
            StatusCode = statusCode,
        };
    }

    public static IActionResult ToResponse(this Error error)
    {
        //if (error is null)
        //{
        //    return new ObjectResult(null)
        //    {
        //        StatusCode = StatusCodes.Status500InternalServerError,
        //    };
        //}

    

        int statusCode = GetStatusCodeFromErrorType(error.Type);

        return new ObjectResult(value: error)
        {
            StatusCode = statusCode,
        };
    }

    private static int GetStatusCodeFromErrorType(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.VALIDATION => StatusCodes.Status400BadRequest,
            ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
            ErrorType.CONFLICT => StatusCodes.Status409Conflict,
            ErrorType.FAILURE => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}