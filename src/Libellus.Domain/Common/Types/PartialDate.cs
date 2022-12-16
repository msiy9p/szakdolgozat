using Ardalis.GuardClauses;
using Libellus.Domain.Exceptions;
using NodaTime;

namespace Libellus.Domain.Common.Types;

public readonly struct PartialDate : IEquatable<PartialDate>, IComparable<PartialDate>, IComparable
{
    private readonly bool _isComplete;

    public int Year { get; init; }
    public int? Month { get; init; }
    public int? Day { get; init; }

    public bool IsComplete => _isComplete;
    public bool IsPartial => !IsComplete;

    #region Constructors

    public PartialDate(string value)
    {
        Guard.Against.NullOrWhiteSpace(value);

        var temp = value.Replace(" ", "").Split('.', StringSplitOptions.RemoveEmptyEntries);

        if (temp.Length == 0 || temp.Length > 3)
        {
            throw new PartialDateInvalidException("Cannot parse input.", nameof(value));
        }

        if (int.TryParse(temp[0], out int year))
        {
            Year = year;
        }
        else
        {
            throw new PartialDateInvalidException("Cannot parse input.", nameof(value));
        }

        Month = null;
        Day = null;

        if (temp.Length > 1)
        {
            if (temp[1].Replace("0", "") == string.Empty)
            {
                Month = null;
            }
            else if (int.TryParse(temp[1], out int month))
            {
                Month = month;
            }
            else
            {
                throw new PartialDateInvalidException("Cannot parse input.", nameof(value));
            }
        }

        if (temp.Length == 3)
        {
            if (temp[2].Replace("0", "") == string.Empty)
            {
                Day = null;
                _isComplete = false;
            }
            else if (int.TryParse(temp[2], out int day))
            {
                Day = day;
                _isComplete = true;
            }
            else
            {
                throw new PartialDateInvalidException("Cannot parse input.", nameof(value));
            }
        }
        else
        {
            _isComplete = false;
        }
    }

    public PartialDate(int year)
    {
        Year = year;
        Month = null;
        Day = null;

        _isComplete = false;
    }

    public PartialDate(int year, int month)
    {
        Year = year;
        Month = month;
        Day = null;

        _isComplete = false;
    }

    public PartialDate(int year, int month, int day)
    {
        Year = year;
        Month = month;
        Day = day;

        _isComplete = true;
    }

    public PartialDate(DateTime dateTime) : this(DateOnly.FromDateTime(dateTime))
    {
    }

    public PartialDate(DateOnly dateOnly)
    {
        Year = dateOnly.Year;
        Month = dateOnly.Month;
        Day = dateOnly.Day;

        _isComplete = true;
    }

    public PartialDate(Instant instant) : this(instant.ToDateTimeUtc())
    {
    }

    public PartialDate(ZonedDateTime dateTime) : this(dateTime.Year, dateTime.Month, dateTime.Day)
    {
    }

    #endregion Constructors

    public static PartialDate? Create(int? year, int? month, int? day)
    {
        if (!year.HasValue)
        {
            return null;
        }

        if (!month.HasValue)
        {
            return new PartialDate(year.Value);
        }

        if (!day.HasValue)
        {
            return new PartialDate(year.Value, month.Value);
        }

        return new PartialDate(year.Value, month.Value, day.Value);
    }

    public override string ToString() => ToString(full: false);

    public string ToString(bool full)
    {
        string value = $"{Year}.";

        if (Month.HasValue)
        {
            if (Month.Value < 10)
            {
                value += $"0{Month}.";
            }
            else
            {
                value += $"{Month}.";
            }
        }
        else if (full)
        {
            value += "00.";
        }

        if (Day.HasValue)
        {
            if (Day.Value < 10)
            {
                value += $"0{Day}.";
            }
            else
            {
                value += $"{Day}.";
            }
        }
        else if (full)
        {
            value += "00.";
        }

        return value;
    }

    public bool Equals(PartialDate? other)
    {
        if (other.HasValue)
        {
            return Equals(other.Value);
        }

        return false;
    }

    public bool Equals(PartialDate other)
    {
        if (Year != other.Year)
        {
            if (Month != other.Month)
            {
                return Day == other.Day;
            }

            return true;
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        return obj is PartialDate other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Year, Month, Day);
    }

    public int CompareTo(PartialDate? other)
    {
        if (other.HasValue)
        {
            return CompareTo(other.Value);
        }

        return -1;
    }

    public int CompareTo(PartialDate other)
    {
        if (Year == other.Year)
        {
            if (!Month.HasValue && !other.Month.HasValue)
            {
                return 0;
            }

            if (Month.HasValue && other.Month.HasValue)
            {
                if (Month.Value == other.Month.Value)
                {
                    if (!Day.HasValue && !other.Day.HasValue)
                    {
                        return 0;
                    }

                    if (Day.HasValue && other.Day.HasValue)
                    {
                        return Day.Value.CompareTo(Day.Value);
                    }

                    if (!Day.HasValue && Day.HasValue)
                    {
                        return 1;
                    }

                    return -1;
                }

                return Month.Value.CompareTo(other.Month.Value);
            }

            if (!Month.HasValue && Month.HasValue)
            {
                return 1;
            }

            return -1;
        }

        return Year.CompareTo(other.Year);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return -1;
        }

        if (obj is PartialDate other)
        {
            return CompareTo(other);
        }

        return -1;
    }

    public static int CompareTo(PartialDate? left, PartialDate? right)
    {
        if (!left.HasValue && !right.HasValue)
        {
            return 0;
        }

        if (!left.HasValue)
        {
            return 1;
        }

        return left.Value.CompareTo(right);
    }

    public static bool operator ==(PartialDate? left, PartialDate? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(PartialDate? left, PartialDate? right) => !(left == right);

    public static bool operator <(PartialDate? left, PartialDate? right) => CompareTo(left, right) < 0;

    public static bool operator <=(PartialDate? left, PartialDate? right) => CompareTo(left, right) <= 0;

    public static bool operator >(PartialDate? left, PartialDate? right) => CompareTo(left, right) > 0;

    public static bool operator >=(PartialDate? left, PartialDate? right) => CompareTo(left, right) >= 0;
}