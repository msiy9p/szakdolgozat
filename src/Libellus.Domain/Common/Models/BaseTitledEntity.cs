using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using NodaTime;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Common.Models;

public abstract class BaseTitledEntity<TKey> : BaseStampedEntity<TKey> where TKey : IEquatable<TKey>
{
    public Title Title { get; private set; }

    protected BaseTitledEntity(TKey id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, Title title) : base(id,
        createdOnUtc, modifiedOnUtc)
    {
        Title = Guard.Against.Null(title);
    }

    protected internal static Result Create(TKey id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        Title title)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc);
        if (result.IsError)
        {
            return result;
        }

        if (title is null)
        {
            return GeneralErrors.InputIsNull.ToInvalidResult();
        }

        return Result.Success();
    }

    public bool ChangeTitle(string title, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!Title.IsValidTitle(title))
        {
            return false;
        }

        Title = new Title(title);

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeTitle(Title title, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (title is null)
        {
            return false;
        }

        Title = title;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public override string ToString() => Title.ToString();

    public class CompareByTitle : Comparer<BaseTitledEntity<TKey>>
    {
        public override int Compare(BaseTitledEntity<TKey>? x, BaseTitledEntity<TKey>? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            var result = new Title.CompareByTitle().Compare(x!.Title, y!.Title);
            if (result != 0)
            {
                return result;
            }

            return new CompareByCreatedOnUtc().Compare(x, y);
        }
    }

    public class CompareByTitleDesc : Comparer<BaseTitledEntity<TKey>>
    {
        public override int Compare(BaseTitledEntity<TKey>? x, BaseTitledEntity<TKey>? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            var result = new Title.CompareByTitleDesc().Compare(x!.Title, y!.Title);
            if (result != 0)
            {
                return result;
            }

            return new CompareByCreatedOnUtcDesc().Compare(x, y);
        }
    }
}