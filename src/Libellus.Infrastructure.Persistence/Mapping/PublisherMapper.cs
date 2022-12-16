using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using DomainPublisher = Libellus.Domain.Entities.Publisher;
using PersistencePublisher = Libellus.Infrastructure.Persistence.DataModels.Publisher;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct PublisherMapper : IMapFrom<PersistencePublisher, Result<DomainPublisher>>,
    IMapFrom<DomainPublisher, PersistencePublisher>, IMapFrom<DomainPublisher, GroupId, PersistencePublisher>
{
    public static Result<DomainPublisher> Map(PersistencePublisher item1)
    {
        return DomainPublisher.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            item1.CreatorId,
            (ShortName)item1.Name);
    }

    public static PersistencePublisher Map(DomainPublisher item)
    {
        return new PersistencePublisher()
        {
            Id = item.Id,
            CreatorId = item.CreatorId,
            Name = item.Name.Value,
            NameNormalized = item.Name.ValueNormalized,
            CreatedOnUtc = item.CreatedOnUtc,
            ModifiedOnUtc = item.ModifiedOnUtc
        };
    }

    public static PersistencePublisher Map(DomainPublisher item1, GroupId item2)
    {
        var publisher = Map(item1);
        publisher.GroupId = item2;

        return publisher;
    }
}