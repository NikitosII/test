using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Orders.Api.Infrastructure;

/// <summary>Строит ответ об ошибках валидации.</summary>
public static class ValidationProblemDetailsFactory
{
    private const string DocumentKey = "";
    private const string EmptyBodyMessage = "Тело запроса отсутствует.";
    private const string MalformedBodyMessage = "Не удалось прочитать тело запроса: проверьте формат JSON и типы значений.";

    /// <summary>Собирает ответ из ошибок.</summary>
    public static ValidationProblemDetails Create(ActionContext context)
    {
        var problemDetails = new ValidationProblemDetails(CollectErrors(context))
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "Ошибка валидации.",
            Status = StatusCodes.Status400BadRequest,
            Instance = context.HttpContext.Request.Path,
        };

        problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

        return problemDetails;
    }

    private static Dictionary<string, string[]> CollectErrors(ActionContext context)
    {
        var bodyParameterNames = context.ActionDescriptor.Parameters
            .Where(parameter => parameter.BindingInfo?.BindingSource == BindingSource.Body)
            .Select(parameter => parameter.Name)
            .ToHashSet(StringComparer.Ordinal);

        return context.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .SelectMany(entry => Describe(context, entry.Key, entry.Value!.Errors, bodyParameterNames))
            .GroupBy(error => error.Key, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Message).Distinct(StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal);
    }

    private static IEnumerable<(string Key, string Message)> Describe(
        ActionContext context,
        string modelStateKey,
        ModelErrorCollection modelErrors,
        HashSet<string> bodyParameterNames)
    {
        if (IsBindingError(modelStateKey, bodyParameterNames))
        {
            return [(DocumentKey, DescribeBody(context))];
        }

        var key = NormalizeKey(modelStateKey);

        return modelErrors.Select(error => (Key: key, Message: error.ErrorMessage));
    }

    // Ключ, начинающийся с '$', — это JSON-путь от System.Text.Json, то есть тело не разобралось.
    private static bool IsBindingError(string modelStateKey, HashSet<string> bodyParameterNames) =>
        string.IsNullOrEmpty(modelStateKey)
        || modelStateKey.StartsWith('$')
        || bodyParameterNames.Contains(modelStateKey);

    private static string DescribeBody(ActionContext context) =>
        context.HttpContext.Request.ContentLength is null or 0 ? EmptyBodyMessage : MalformedBodyMessage;

    private static string NormalizeKey(string key) =>
        string.Join('.', key.Split('.').Select(JsonNamingPolicy.CamelCase.ConvertName));
}
