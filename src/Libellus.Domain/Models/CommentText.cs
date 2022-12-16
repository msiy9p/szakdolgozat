using Ardalis.GuardClauses;
using Libellus.Domain.Utilities;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Models;

public sealed class CommentText
{
    public const int MaxLength = 10000;

    public string Value { get; init; }

    public CommentText(string value)
    {
        Guard.Against.NullOrWhiteSpace(value);
        if (value.Length > MaxLength)
        {
            throw new ArgumentException("Longer then max length.", nameof(value));
        }

        Value = value;
    }

    public static Result<CommentText> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return CommentTextErrors.InvalidCommentText.ToInvalidResult<CommentText>();
        }

        if (value.Length > MaxLength)
        {
            return CommentTextErrors.CommentTextTooLong.ToInvalidResult<CommentText>();
        }

        return Result<CommentText>.Success(new CommentText(value));
    }

    public static Result Probe(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return CommentTextErrors.InvalidCommentText.ToInvalidResult();
        }

        if (value.Length > MaxLength)
        {
            return CommentTextErrors.CommentTextTooLong.ToInvalidResult();
        }

        return Result.Success();
    }

    public static bool IsValidCommentText(string value)
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

    public static implicit operator string(CommentText value) => value.Value;
    public static explicit operator CommentText(string item) => new(item);

    public override string ToString() => Value;
}