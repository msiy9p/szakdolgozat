using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using DomainFormat = Libellus.Domain.Entities.Format;
using PersistenceFormat = Libellus.Infrastructure.Persistence.DataModels.Format;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct FormatMapper : IMapFrom<PersistenceFormat, Result<DomainFormat>>,
    IMapFrom<DomainFormat, PersistenceFormat>, IMapFrom<DomainFormat, GroupId, PersistenceFormat>
{
    public static Result<DomainFormat> Map(PersistenceFormat item1)
    {
        return DomainFormat.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            item1.CreatorId,
            (ShortName)item1.Name,
            item1.IsDigital);
    }

    public static PersistenceFormat Map(DomainFormat item)
    {
        return new PersistenceFormat()
        {
            Id = item.Id,
            CreatorId = item.CreatorId,
            Name = item.Name.Value,
            NameNormalized = item.Name.ValueNormalized,
            IsDigital = item.IsDigital,
            CreatedOnUtc = item.CreatedOnUtc,
            ModifiedOnUtc = item.ModifiedOnUtc
        };
    }

    public static PersistenceFormat Map(DomainFormat item1, GroupId item2)
    {
        var format = Map(item1);
        format.GroupId = item2;

        return format;
    }
}