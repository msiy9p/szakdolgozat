using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using DomainTag = Libellus.Domain.Entities.Tag;
using PersistenceTag = Libellus.Infrastructure.Persistence.DataModels.Tag;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct TagMapper : IMapFrom<PersistenceTag, Result<DomainTag>>, IMapFrom<DomainTag, PersistenceTag>,
    IMapFrom<DomainTag, GroupId, PersistenceTag>
{
    public static Result<DomainTag> Map(PersistenceTag item)
    {
        return DomainTag.Create(
            item.Id,
            item.CreatedOnUtc,
            item.ModifiedOnUtc,
            item.CreatorId,
            (ShortName)item.Name);
    }

    public static PersistenceTag Map(DomainTag item)
    {
        return new PersistenceTag()
        {
            Id = item.Id,
            CreatorId = item.CreatorId,
            Name = item.Name.Value,
            NameNormalized = item.Name.ValueNormalized,
            CreatedOnUtc = item.CreatedOnUtc,
            ModifiedOnUtc = item.ModifiedOnUtc
        };
    }

    public static PersistenceTag Map(DomainTag item1, GroupId item2)
    {
        var tag = Map(item1);
        tag.GroupId = item2;

        return tag;
    }
}