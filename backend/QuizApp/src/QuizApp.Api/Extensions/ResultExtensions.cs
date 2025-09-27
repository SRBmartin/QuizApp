using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Application.Common.Result;

namespace QuizApp.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this ControllerBase controller, Result result, int successStatusCode = StatusCodes.Status204NoContent)
    {
        if (result.IsSuccess)
            return successStatusCode == StatusCodes.Status204NoContent ?
                controller.NoContent() :
                controller.StatusCode(successStatusCode);

        return controller.StatusCode(MapStatus(result.Error), new
        {
            error = result.Error.Code,
            message = result.Error.Message
        });
    }

    public static IActionResult ToActionResult<T>(
        this ControllerBase controller,
        Result<T> result,
        Func<T, object>? projector = null,
        int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsSuccess)
        {
            if (successStatusCode == StatusCodes.Status204NoContent)
                return controller.NoContent();

            if (typeof(T) == typeof(Unit) && successStatusCode == StatusCodes.Status200OK)
                return controller.NoContent();

            var payload = projector is null ? (object?)result.Value : projector(result.Value!);
            return controller.StatusCode(successStatusCode, payload);
        }

        return controller.StatusCode(MapStatus(result.Error), new
        {
            error = result.Error.Code,
            message = result.Error.Message
        });
    }

    #region Helpers

    private static int MapStatus(Error error)
    {
        var code = (error.Code ?? string.Empty).ToLowerInvariant();

        if (code.Contains("unauthorized") || code.StartsWith("auth.")) return StatusCodes.Status401Unauthorized;
        if (code.Contains("forbidden")) return StatusCodes.Status403Forbidden;
        if (code.Contains("not_found") || code.Contains("notfound")) return StatusCodes.Status404NotFound;
        if (code.Contains("conflict") || code.Contains("already")) return StatusCodes.Status409Conflict;
        if (code.Contains("validation")) return StatusCodes.Status422UnprocessableEntity;
        if (code.Contains("too_many") || code.Contains("rate_limit")) return StatusCodes.Status429TooManyRequests;

        return StatusCodes.Status400BadRequest;
    }

    #endregion

}
