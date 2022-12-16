using Libellus.Domain.Enums;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Application.Enums;

public static class SortOrderExtensions
{
    public static bool IsDefined(SortOrder sortOrder) =>
        sortOrder switch
        {
            SortOrder.Ascending => true,
            SortOrder.Descending => true,
            _ => false
        };

    public static string ToString(SortOrder sortOrder) =>
        sortOrder switch
        {
            SortOrder.Ascending => nameof(SortOrder.Ascending),
            SortOrder.Descending => nameof(SortOrder.Descending),
            _ => string.Empty
        };

    public static Result<SortOrder> FromString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<SortOrder>.Error(GeneralErrors.StringNullOrWhiteSpace);
        }

        return value!.ToLowerInvariant() switch
        {
            "ascending" => Result<SortOrder>.Success(SortOrder.Ascending),
            "descending" => Result<SortOrder>.Success(SortOrder.Descending),
            _ => Result<SortOrder>.Error(SortOrderErrors.InvalidSortOrder),
        };
    }
}