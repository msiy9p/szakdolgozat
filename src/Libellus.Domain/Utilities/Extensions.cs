using Ardalis.GuardClauses;
using Libellus.Domain.Common.Errors;
using Libellus.Domain.Models;
using System.Text;
using Libellus.Domain.Common.Interfaces.Errors;
using Microsoft.Extensions.Logging;

namespace Libellus.Domain.Utilities;

public static class Extensions
{
    public static string ToNormalizedUpperInvariant(this string input)
        => ToNormalizedUpperInvariant(input, NormalizationForm.FormC);

    public static string ToNormalizedUpperInvariant(this string input, NormalizationForm normalizationForm)
    {
        Guard.Against.Null(input);

        return input.ToUpperInvariant().Normalize(normalizationForm);
    }

    public static Result ToErrorResult(this Error error)
    {
        Guard.Against.Null(error);

        return Result.Error(error);
    }

    public static Result<T> ToErrorResult<T>(this Error error)
    {
        Guard.Against.Null(error);

        return Result<T>.Error(error);
    }

    public static Result ToInvalidResult(this Error error)
    {
        Guard.Against.Null(error);

        return Result.Invalid(error);
    }

    public static Result<T> ToInvalidResult<T>(this Error error)
    {
        Guard.Against.Null(error);

        return Result<T>.Invalid(error);
    }

    public static Result<T> ToResult<T>(this T value)
    {
        return Result<T>.Success(value);
    }

    public static Result<ICollection<T>> ToResultCollection<T>(this IList<T> value)
    {
        return Result<ICollection<T>>.Success(value);
    }

    public static void Log(this ILogger logger, LogLevel logLevel, IError error)
    {
        logger.Log(logLevel, "{ErrorId}: {Message}", error.ErrorId, error.Message);
    }

    public static void Log(this ILogger logger, LogLevel logLevel, IEnumerable<IError> errors)
    {
        foreach (var error in errors)
        {
            logger.Log(logLevel, error);
        }
    }

    public static void LogDebug(this ILogger logger, IError error) =>
        logger.Log(LogLevel.Debug, error);

    public static void LogTrace(this ILogger logger, IError error) =>
        logger.Log(LogLevel.Trace, error);

    public static void LogInformation(this ILogger logger, IError error) =>
        logger.Log(LogLevel.Information, error);

    public static void LogWarning(this ILogger logger, IError error) =>
        logger.Log(LogLevel.Warning, error);

    public static void LogError(this ILogger logger, IError error) =>
        logger.Log(LogLevel.Error, error);

    public static void LogCritical(this ILogger logger, IError error) =>
        logger.Log(LogLevel.Critical, error);

    public static void LogDebug(this ILogger logger, IEnumerable<IError> errors) =>
        logger.Log(LogLevel.Debug, errors);

    public static void LogTrace(this ILogger logger, IEnumerable<IError> errors) =>
        logger.Log(LogLevel.Trace, errors);

    public static void LogInformation(this ILogger logger, IEnumerable<IError> errors) =>
        logger.Log(LogLevel.Information, errors);

    public static void LogWarning(this ILogger logger, IEnumerable<IError> errors) =>
        logger.Log(LogLevel.Warning, errors);

    public static void LogError(this ILogger logger, IEnumerable<IError> errors) =>
        logger.Log(LogLevel.Error, errors);

    public static void LogCritical(this ILogger logger, IEnumerable<IError> errors) =>
        logger.Log(LogLevel.Critical, errors);
}