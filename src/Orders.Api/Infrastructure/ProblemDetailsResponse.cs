using Microsoft.AspNetCore.Mvc;

namespace Orders.Api.Infrastructure;

public static class ProblemDetailsResponse
{
    /// <summary>Тип содержимого для ответов об ошибках.</summary>
    public const string ContentType = "application/problem+json";

    /// <summary>Пишет проблему в ответ, выставляя код состояния и тип содержимого.</summary>
    public static Task WriteAsync(HttpContext httpContext, ProblemDetails problemDetails, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        return httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            problemDetails.GetType(),
            options: null,
            ContentType,
            cancellationToken);
    }
}

public sealed class ProblemDetailsResult(ProblemDetails problemDetails) : IActionResult
{
    public Task ExecuteResultAsync(ActionContext context) =>
        ProblemDetailsResponse.WriteAsync(context.HttpContext, problemDetails, context.HttpContext.RequestAborted);
}
