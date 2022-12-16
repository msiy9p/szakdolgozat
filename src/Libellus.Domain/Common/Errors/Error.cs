using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Errors;

namespace Libellus.Domain.Common.Errors;

public class Error : IError, IEquatable<Error>
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public string ErrorId { get; init; }
    public string Message { get; init; }

    public Error(string errorId, string message)
    {
        ErrorId = Guard.Against.Null(errorId);
        Message = Guard.Against.Null(message);
    }

    public bool Equals(IError? other) => Equals(other as object);

    public bool Equals(Error? other) => Equals(other as object);

    public bool Equals(IError<string>? other) => Equals(other as object);

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (Error)obj;
        return ErrorId.Equals(other.ErrorId);
    }

    public override int GetHashCode() => ErrorId.GetHashCode();

    public static implicit operator string(Error error) => error.ErrorId;

    protected static bool EqualOperator(Error left, Error right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }

        return ReferenceEquals(left, right) || left!.Equals(right);
    }

    protected static bool NotEqualOperator(Error left, Error right)
    {
        return !EqualOperator(left, right);
    }

    public static bool operator ==(Error one, Error two) => EqualOperator(one, two);

    public static bool operator !=(Error one, Error two) => NotEqualOperator(one, two);
}