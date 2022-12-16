using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using DomainLabel = Libellus.Domain.Entities.Label;
using PersistenceLabel = Libellus.Infrastructure.Persistence.DataModels.Label;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct LabelMapper : IMapFrom<PersistenceLabel, Result<DomainLabel>>,
    IMapFrom<DomainLabel, PersistenceLabel>, IMapFrom<DomainLabel, GroupId, PersistenceLabel>
{
    public static Result<DomainLabel> Map(PersistenceLabel item1)
    {
        return DomainLabel.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            (ShortName)item1.Name);
    }

    public static PersistenceLabel Map(DomainLabel item)
    {
        return new PersistenceLabel()
        {
            Id = item.Id,
            Name = item.Name.Value,
            NameNormalized = item.Name.ValueNormalized,
            CreatedOnUtc = item.CreatedOnUtc,
            ModifiedOnUtc = item.ModifiedOnUtc
        };
    }

    public static PersistenceLabel Map(DomainLabel item1, GroupId item2)
    {
        var label = Map(item1);
        label.GroupId = item2;

        return label;
    }
}