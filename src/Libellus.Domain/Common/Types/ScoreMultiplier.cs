using Libellus.Domain.Exceptions;
using Libellus.Domain.Models;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Common.Types;

public readonly struct ScoreMultiplier : IEquatable<ScoreMultiplier>, IComparable, IComparable<ScoreMultiplier>
{
    public const decimal ScoreMultiplierMin = 0;
    public const decimal ScoreMultiplierMax = 2;
    public const decimal ScoreMultiplierDefault = 1;

    public decimal Value { get; init; }

    public ScoreMultiplier(decimal value = ScoreMultiplierDefault)
    {
        if (!IsScoreMultiplierValid(value))
        {
            throw new ScoreMultiplierInvalidException("Multiplier is not within acceptable range.", nameof(value));
        }

        Value = decimal.Round(value, 3, MidpointRounding.AwayFromZero);
    }

    public static Result<ScoreMultiplier> Create(decimal value)
    {
        if (!IsScoreMultiplierValid(value))
        {
            return Result<ScoreMultiplier>.Invalid(ScoreMultiplierErrors.InvalidScoreMultiplier);
        }

        return Result<ScoreMultiplier>.Success(new ScoreMultiplier(value));
    }

    public static ScoreMultiplier? Convert(decimal? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        if (!IsScoreMultiplierValid(value.Value))
        {
            return null;
        }

        return new ScoreMultiplier(value.Value);
    }

    public static bool IsScoreMultiplierValid(decimal scoreMultiplier) =>
        scoreMultiplier >= ScoreMultiplierMin && scoreMultiplier <= ScoreMultiplierMax;

    public override int GetHashCode() => Value.GetHashCode();

    public override bool Equals(object? obj) => obj is ScoreMultiplier other && Equals(other);

    public bool Equals(ScoreMultiplier other) => Value.Equals(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is ScoreMultiplier value)
        {
            return CompareTo(value);
        }

        return 1;
    }

    public int CompareTo(ScoreMultiplier other) => Value.CompareTo(other.Value);

    public static implicit operator decimal(ScoreMultiplier value) => value.Value;

    public static bool operator ==(ScoreMultiplier left, ScoreMultiplier right) => left.Equals(right);

    public static bool operator !=(ScoreMultiplier left, ScoreMultiplier right) => !(left == right);

    public static bool operator <(ScoreMultiplier left, ScoreMultiplier right) => left.CompareTo(right) < 0;

    public static bool operator <=(ScoreMultiplier left, ScoreMultiplier right) => left.CompareTo(right) <= 0;

    public static bool operator >(ScoreMultiplier left, ScoreMultiplier right) => left.CompareTo(right) > 0;

    public static bool operator >=(ScoreMultiplier left, ScoreMultiplier right) => left.CompareTo(right) >= 0;
}