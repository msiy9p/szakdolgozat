using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using NodaTime;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Common.Models;

public abstract class BaseNamedEntity<TKey> : BaseStampedEntity<TKey> where TKey : IEquatable<TKey>
{
    public Name Name { get; private set; }

    protected BaseNamedEntity(TKey id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, Name name) : base(id,
        createdOnUtc, modifiedOnUtc)
    {
        Name = Guard.Against.Null(name);
    }

    protected internal static Result Create(TKey id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, Name name)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc);
        if (result.IsError)
        {
            return result;
        }

        if (name is null)
        {
            return GeneralErrors.InputIsNull.ToInvalidResult();
        }

        return Result.Success();
    }

    public bool ChangeName(string name, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!Name.IsValidName(name))
        {
            return false;
        }

        Name = new Name(name);

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeName(Name name, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (name is null)
        {
            return false;
        }

        Name = name;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public override string ToString() => Name.ToString();

    public class CompareByName : Comparer<BaseNamedEntity<TKey>>
    {
        public override int Compare(BaseNamedEntity<TKey>? x, BaseNamedEntity<TKey>? y)
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

            var result = new Name.CompareByName().Compare(x!.Name, y!.Name);
            if (result != 0)
            {
                return result;
            }

            return new CompareByCreatedOnUtc().Compare(x, y);
        }
    }

    public class CompareByNameDesc : Comparer<BaseNamedEntity<TKey>>
    {
        public override int Compare(BaseNamedEntity<TKey>? x, BaseNamedEntity<TKey>? y)
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

            var result = new Name.CompareByNameDesc().Compare(x!.Name, y!.Name);
            if (result != 0)
            {
                return result;
            }

            return new CompareByCreatedOnUtcDesc().Compare(x, y);
        }
    }
}