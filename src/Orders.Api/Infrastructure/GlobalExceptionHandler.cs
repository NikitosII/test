using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Exceptions;
using Orders.Domain.Exceptions;

namespace Orders.Api.Infrastructure;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            OrderNotFoundException notFound => Create(StatusCodes.Status404NotFound, "Заказ не найден.", notFound.Message),
            DomainException domain => Create(StatusCodes.Status400BadRequest, "Некорректные данные заказа.", domain.Message),
            _ => Create(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера.", "Повторите попытку позже."),
        };

        if (problemDetails.Status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Необработанное исключение при обработке {Method} {Path}.",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning("Запрос {Method} {Path} отклонён: {Reason}",
                httpContext.Request.Method, httpContext.Request.Path, exception.Message);
        }

        problemDetails.Instance = httpContext.Request.Path;
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        await ProblemDetailsResponse.WriteAsync(httpContext, problemDetails, cancellationToken);
        return true;
    }

    private static ProblemDetails Create(int status, string title, string detail) =>
        new()
        {
            Status = status,
            Title = title,
            Detail = detail,
        };
}
