using Ardalis.GuardClauses;
using Libellus.Domain.Utilities;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Models;

public sealed class DescriptionText
{
    public const int MaxLength = 500;

    public string Value { get; init; }

    public DescriptionText(string value)
    {
        Guard.Against.NullOrWhiteSpace(value);
        if (value.Length > MaxLength)
        {
            throw new ArgumentException("Longer then max length.", nameof(value));
        }

        Value = value;
    }

    public static Result<DescriptionText> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DescriptionTextErrors.InvalidDescriptionText.ToInvalidResult<DescriptionText>();
        }

        if (value.Length > MaxLength)
        {
            return DescriptionTextErrors.DescriptionTextTooLong.ToInvalidResult<DescriptionText>();
        }

        return Result<DescriptionText>.Success(new DescriptionText(value));
    }

    public static Result Probe(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DescriptionTextErrors.InvalidDescriptionText.ToInvalidResult();
        }

        if (value.Length > MaxLength)
        {
            return DescriptionTextErrors.DescriptionTextTooLong.ToInvalidResult();
        }

        return Result.Success();
    }

    public static bool IsValidDescriptionText(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (value.Length > MaxLength)
        {
            return false;
        }

        return true;
    }

    public static implicit operator string(DescriptionText item) => item.Value;
    public static explicit operator DescriptionText(string item) => new(item);

    public override string ToString() => Value;
}