using Libellus.Domain.Enums;
using Libellus.Domain.Exceptions;
using System.Runtime.CompilerServices;
using JetBrainsInvokerParameterNameAttribute = JetBrains.Annotations.InvokerParameterNameAttribute;
using JetBrainsNotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace Ardalis.GuardClauses;

public static partial class CustomGuardClauseExtensions
{
    /// <summary>
    /// Throws an <see cref="ImageFormatNotSupportedException" /> if <paramref name="input"/> is not a valid enum value.
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="parameterName"></param>
    /// /// <param name="message">Optional. Custom error message</param>
    /// <returns><paramref name="input" /> if the value is not out of range.</returns>
    /// <exception cref="ImageFormatNotSupportedException"></exception>
    public static ImageFormat ImageFormatOutOfRange([JetBrainsNotNull] this IGuardClause guardClause,
        ImageFormat input,
        [JetBrainsNotNull] [JetBrainsInvokerParameterName] [CallerArgumentExpression("input")]
        string? parameterName = null,
        string? message = null)
    {
        if (!ImageFormatExtensions.IsDefined(input))
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ImageFormatNotSupportedException(parameterName, Convert.ToInt32(input), typeof(ImageFormat));
            }

            throw new ImageFormatNotSupportedException(message);
        }

        return input;
    }

    /// <summary>
    /// Throws an <see cref="ImageFormatNotSupportedException" /> if <paramref name="input"/> is not a valid enum value.
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="parameterName"></param>
    /// /// <param name="message">Optional. Custom error message</param>
    /// <returns><paramref name="input" /> if the value is not out of range.</returns>
    /// <exception cref="ImageFormatNotSupportedException"></exception>
    public static ImageSizeWidth ImageSizeWidthOutOfRange([JetBrainsNotNull] this IGuardClause guardClause,
        ImageSizeWidth input,
        [JetBrainsNotNull] [JetBrainsInvokerParameterName] [CallerArgumentExpression("input")]
        string? parameterName = null,
        string? message = null)
    {
        if (!ImageSizeWidthExtensions.IsDefined(input))
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ImageSizeNotSupportedException(parameterName, Convert.ToInt32(input), typeof(ImageSizeWidth));
            }

            throw new ImageSizeNotSupportedException(message);
        }

        return input;
    }

    /// <summary>
    /// Throws an <see cref="ImageFormatNotSupportedException" /> if <paramref name="input"/> is not a valid enum value.
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="parameterName"></param>
    /// /// <param name="message">Optional. Custom error message</param>
    /// <returns><paramref name="input" /> if the value is not out of range.</returns>
    /// <exception cref="ImageFormatNotSupportedException"></exception>
    public static ImageSizeHeight ImageSizeHeightOutOfRange([JetBrainsNotNull] this IGuardClause guardClause,
        ImageSizeHeight input,
        [JetBrainsNotNull] [JetBrainsInvokerParameterName] [CallerArgumentExpression("input")]
        string? parameterName = null,
        string? message = null)
    {
        if (!ImageSizeHeightExtensions.IsDefined(input))
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ImageSizeNotSupportedException(parameterName, Convert.ToInt32(input),
                    typeof(ImageSizeHeight));
            }

            throw new ImageSizeNotSupportedException(message);
        }

        return input;
    }
}