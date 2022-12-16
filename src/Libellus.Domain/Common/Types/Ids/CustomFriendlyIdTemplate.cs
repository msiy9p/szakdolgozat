﻿using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct CustomFriendlyIdTemplate : IFriendlyIdType<string>, IComparable<CustomFriendlyIdTemplate>,
    IEquatable<CustomFriendlyIdTemplate>
{
    public const int Length = 10;
    private const string Alphabet = "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public string Value { get; init; }

    public CustomFriendlyIdTemplate()
    {
        Value = Nanoid.Nanoid.Generate(Alphabet, Length);
    }

    public CustomFriendlyIdTemplate(string value)
    {
        Guard.Against.NullOrEmpty(value);
        if (value.Length != Length)
        {
            throw new ArgumentException("Length not valid.", nameof(value));
        }

        foreach (var item in value)
        {
            if (!Alphabet.Contains(item))
            {
                throw new ArgumentException("Not in alphabet.", nameof(value));
            }
        }

        Value = value;
    }

    public static CustomFriendlyIdTemplate Create() => new(Nanoid.Nanoid.Generate(Alphabet, Length));

    public static CustomFriendlyIdTemplate? Convert(string? value)
    {
        if (IsValid(value))
        {
            return new CustomFriendlyIdTemplate(value!);
        }

        return null;
    }

    public int CompareTo(CustomFriendlyIdTemplate other) =>
        string.Compare(Value, other.Value, StringComparison.Ordinal);

    public bool Equals(CustomFriendlyIdTemplate other) => Value.Equals(other.Value);

    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (value.Length != Length)
        {
            return false;
        }

        foreach (var item in value)
        {
            if (!Alphabet.Contains(item))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is CustomFriendlyIdTemplate other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool operator ==(CustomFriendlyIdTemplate a, CustomFriendlyIdTemplate b) => a.CompareTo(b) == 0;

    public static bool operator !=(CustomFriendlyIdTemplate a, CustomFriendlyIdTemplate b) => !(a == b);

    public static bool operator <(CustomFriendlyIdTemplate left, CustomFriendlyIdTemplate right) =>
        left.CompareTo(right) < 0;

    public static bool operator <=(CustomFriendlyIdTemplate left, CustomFriendlyIdTemplate right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >(CustomFriendlyIdTemplate left, CustomFriendlyIdTemplate right) =>
        left.CompareTo(right) > 0;

    public static bool operator >=(CustomFriendlyIdTemplate left, CustomFriendlyIdTemplate right) =>
        left.CompareTo(right) >= 0;
}