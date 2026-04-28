using Microsoft.AspNetCore.Mvc;
using Photography.SharedKernel;

namespace Photography.Web;

/// <summary>
/// Maps a domain <see cref="Result"/> / <see cref="Result{T}"/> to an appropriate HTTP response.
/// Keeps controllers free of any branching logic on result kind.
/// </summary>
public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase ctrl)
        => result.Success
            ? ctrl.NoContent()
            : MapFailure(ctrl, result.ErrorKind, result.Error);

    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase ctrl, Func<T, IActionResult>? onSuccess = null)
    {
        if (result.Success)
            return onSuccess is null ? ctrl.Ok(result.Value) : onSuccess(result.Value!);
        return MapFailure(ctrl, result.ErrorKind, result.Error);
    }

    private static IActionResult MapFailure(ControllerBase ctrl, ResultErrorKind kind, string? error) => kind switch
    {
        ResultErrorKind.NotFound => ctrl.NotFound(new { error }),
        ResultErrorKind.Conflict => ctrl.Conflict(new { error }),
        ResultErrorKind.Unauthorized => ctrl.Unauthorized(new { error }),
        ResultErrorKind.Forbidden => ctrl.StatusCode(StatusCodes.Status403Forbidden, new { error }),
        _ => ctrl.BadRequest(new { error }),
    };
}
