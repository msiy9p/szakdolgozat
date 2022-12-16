using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using DomainWarningTag = Libellus.Domain.Entities.WarningTag;
using PersistenceWarningTag = Libellus.Infrastructure.Persistence.DataModels.WarningTag;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct WarningTagMapper : IMapFrom<PersistenceWarningTag, Result<DomainWarningTag>>,
    IMapFrom<DomainWarningTag, PersistenceWarningTag>, IMapFrom<DomainWarningTag, GroupId, PersistenceWarningTag>
{
    public static Result<DomainWarningTag> Map(PersistenceWarningTag item)
    {
        return DomainWarningTag.Create(
            item.Id,
            item.CreatedOnUtc,
            item.ModifiedOnUtc,
            item.CreatorId,
            (ShortName)item.Name);
    }

    public static PersistenceWarningTag Map(DomainWarningTag item)
    {
        return new PersistenceWarningTag()
        {
            Id = item.Id,
            CreatorId = item.CreatorId,
            Name = item.Name.Value,
            NameNormalized = item.Name.ValueNormalized,
            CreatedOnUtc = item.CreatedOnUtc,
            ModifiedOnUtc = item.ModifiedOnUtc
        };
    }

    public static PersistenceWarningTag Map(DomainWarningTag item1, GroupId item2)
    {
        var tag = Map(item1);
        tag.GroupId = item2;

        return tag;
    }
}