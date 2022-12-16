using Libellus.Application.Enums;
using Libellus.Application.Exceptions;
using System.Runtime.CompilerServices;
using JetBrainsInvokerParameterNameAttribute = JetBrains.Annotations.InvokerParameterNameAttribute;
using JetBrainsNotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace Ardalis.GuardClauses;

public static partial class CustomGuardClauseExtensions
{
    /// <summary>
    /// Throws an <see cref="PaginationItemCountNotSupportedException" /> if <paramref name="input"/> is not a valid enum value.
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="parameterName"></param>
    /// /// <param name="message">Optional. Custom error message</param>
    /// <returns><paramref name="input" /> if the value is not out of range.</returns>
    /// <exception cref="PaginationItemCountNotSupportedException"></exception>
    public static PaginationItemCount PaginationItemCountOutOfRange([JetBrainsNotNull] this IGuardClause guardClause,
        PaginationItemCount input,
        [JetBrainsNotNull] [JetBrainsInvokerParameterName] [CallerArgumentExpression("input")]
        string? parameterName = null,
        string? message = null)
    {
        if (!PaginationItemCountExtensions.IsDefined(input))
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new PaginationItemCountNotSupportedException(parameterName, Convert.ToInt32(input),
                    typeof(PaginationItemCount));
            }

            throw new PaginationItemCountNotSupportedException(message);
        }

        return input;
    }
}