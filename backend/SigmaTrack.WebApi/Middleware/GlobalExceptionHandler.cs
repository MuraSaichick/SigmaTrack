using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SigmaTrack.Domain.Exceptions;

namespace SigmaTrack.WebApi.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Произошла необработанная ошибка: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            ValidationException validationEx => CreateValidationProblemDetails(validationEx),
            DomainException domainEx => CreateProblemDetails(StatusCodes.Status400BadRequest, "Ошибка бизнес-логики", domainEx.Message),
            ArgumentException argEx => CreateProblemDetails(StatusCodes.Status400BadRequest, "Некорректный аргумент", argEx.Message),
            KeyNotFoundException knfEx => CreateProblemDetails(StatusCodes.Status404NotFound, "Ресурс не найден", knfEx.Message),
            UnauthorizedAccessException uaeEx => CreateProblemDetails(StatusCodes.Status401Unauthorized, "Нет доступа", uaeEx.Message),
            _ => CreateProblemDetails(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера", "Произошла непредвиденная ошибка на сервере.")
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(int status, string title, string detail) =>
        new() { Status = status, Title = title, Detail = detail };

    private static HttpValidationProblemDetails CreateValidationProblemDetails(ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return new HttpValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Ошибка валидации данных",
            Detail = "Один или несколько параметров запроса не прошли проверку."
        };
    }
}