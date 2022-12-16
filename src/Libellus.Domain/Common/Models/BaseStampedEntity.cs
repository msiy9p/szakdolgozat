using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Exceptions;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using NodaTime;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Common.Models;

public abstract class BaseStampedEntity<TKey> : BaseEntity<TKey> where TKey : IEquatable<TKey>
{
    public ZonedDateTime CreatedOnUtc { get; init; }
    public ZonedDateTime ModifiedOnUtc { get; private set; }

    protected BaseStampedEntity(TKey id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc) : base(id)
    {
        if (!Utilities.Utilities.DoesPrecede(createdOnUtc, modifiedOnUtc))
        {
            throw new CreationModificationDateTimeException();
        }

        CreatedOnUtc = createdOnUtc;
        ModifiedOnUtc = modifiedOnUtc;
    }

    protected internal static Result Create(TKey id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc)
    {
        var result = Create(id);
        if (result.IsError)
        {
            return result;
        }

        if (!Utilities.Utilities.DoesPrecede(createdOnUtc, modifiedOnUtc))
        {
            return StampedEntityErrors.CreationModificationDateTimeError.ToInvalidResult();
        }

        return Result.Success();
    }

    protected void UpdateModifiedOnUtc(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        var datetime = dateTimeProvider.UtcNow;
        if (!Utilities.Utilities.DoesPrecede(CreatedOnUtc, datetime))
        {
            throw new CreationModificationDateTimeException();
        }

        ModifiedOnUtc = datetime;
    }

    public class CompareByCreatedOnUtc : Comparer<BaseStampedEntity<TKey>>
    {
        public override int Compare(BaseStampedEntity<TKey>? x, BaseStampedEntity<TKey>? y)
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

            return ZonedDateTime.Comparer.Instant.Compare(x!.CreatedOnUtc, y!.CreatedOnUtc);
        }
    }

    public class CompareByCreatedOnUtcDesc : Comparer<BaseStampedEntity<TKey>>
    {
        public override int Compare(BaseStampedEntity<TKey>? x, BaseStampedEntity<TKey>? y)
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

            return ZonedDateTime.Comparer.Instant.Compare(x!.CreatedOnUtc, y!.CreatedOnUtc) * -1;
        }
    }

    public class CompareByModifiedOnUtc : Comparer<BaseStampedEntity<TKey>>
    {
        public override int Compare(BaseStampedEntity<TKey>? x, BaseStampedEntity<TKey>? y)
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

            return ZonedDateTime.Comparer.Instant.Compare(x!.ModifiedOnUtc, y!.ModifiedOnUtc);
        }
    }

    public class CompareByModifiedOnUtcDesc : Comparer<BaseStampedEntity<TKey>>
    {
        public override int Compare(BaseStampedEntity<TKey>? x, BaseStampedEntity<TKey>? y)
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

            return ZonedDateTime.Comparer.Instant.Compare(x!.ModifiedOnUtc, y!.ModifiedOnUtc) * -1;
        }
    }
}