using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using DomainGroup = Libellus.Domain.Entities.Group;
using DomainLabel = Libellus.Domain.Entities.Label;
using DomainPost = Libellus.Domain.Entities.Post;
using DomainShelf = Libellus.Domain.Entities.Shelf;
using PersistenceGroup = Libellus.Infrastructure.Persistence.DataModels.Group;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct GroupMapper : IMapFrom<PersistenceGroup, Result<DomainGroup>>,
    IMapFrom<PersistenceGroup, IEnumerable<DomainPost>, Result<DomainGroup>>, IMapFrom<DomainGroup, PersistenceGroup>
{
    public static Result<DomainGroup> Map(PersistenceGroup item1)
    {
        if (item1.Labels.Count > 0)
        {
            var labels = new List<DomainLabel>(item1.Labels.Count);
            foreach (var itemLabel in item1.Labels)
            {
                var mapResult = LabelMapper.Map(itemLabel);
                if (mapResult.IsSuccess)
                {
                    labels.Add(mapResult.Value!);
                }
            }

            return DomainGroup.Create(
                item1.Id,
                item1.CreatedOnUtc,
                item1.ModifiedOnUtc,
                new GroupFriendlyId(item1.FriendlyId),
                (Name)item1.Name,
                (item1.Description is null ? null : (DescriptionText)item1.Description),
                item1.IsPrivate,
                labels);
        }

        return DomainGroup.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new GroupFriendlyId(item1.FriendlyId),
            (Name)item1.Name,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            item1.IsPrivate);
    }

    public static Result<DomainGroup> Map(PersistenceGroup item1, IEnumerable<DomainPost> item2)
    {
        if (item1.Labels.Count > 0)
        {
            var labels = new List<DomainLabel>(item1.Labels.Count);
            foreach (var itemLabel in item1.Labels)
            {
                var mapResult = LabelMapper.Map(itemLabel);
                if (mapResult.IsSuccess)
                {
                    labels.Add(mapResult.Value!);
                }
            }

            return DomainGroup.Create(
                item1.Id,
                item1.CreatedOnUtc,
                item1.ModifiedOnUtc,
                new GroupFriendlyId(item1.FriendlyId),
                (Name)item1.Name,
                (item1.Description is null ? null : (DescriptionText)item1.Description),
                item1.IsPrivate,
                Array.Empty<DomainShelf>(),
                labels,
                item2);
        }

        return DomainGroup.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new GroupFriendlyId(item1.FriendlyId),
            (Name)item1.Name,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            item1.IsPrivate,
            Array.Empty<DomainShelf>(),
            Array.Empty<DomainLabel>(),
            item2);
    }

    public static PersistenceGroup Map(DomainGroup item1)
    {
        return new PersistenceGroup()
        {
            Id = item1.Id,
            FriendlyId = item1.FriendlyId.Value,
            Name = item1.Name.Value,
            NameNormalized = item1.Name.ValueNormalized,
            Description = item1.Description?.Value,
            IsPrivate = item1.IsPrivate,
            CreatedOnUtc = item1.CreatedOnUtc,
            ModifiedOnUtc = item1.ModifiedOnUtc,
        };
    }
}