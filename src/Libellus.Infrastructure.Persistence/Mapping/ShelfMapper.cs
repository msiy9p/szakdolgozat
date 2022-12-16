using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using DomainBook = Libellus.Domain.Entities.Book;
using DomainShelf = Libellus.Domain.Entities.Shelf;
using PersistenceShelf = Libellus.Infrastructure.Persistence.DataModels.Shelf;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct ShelfMapper : IMapFrom<PersistenceShelf, UserVm?, Result<DomainShelf>>,
    IMapFrom<PersistenceShelf, UserVm?, ICollection<DomainBook>, Result<DomainShelf>>,
    IMapFrom<DomainShelf, PersistenceShelf>,
    IMapFrom<DomainShelf, GroupId, PersistenceShelf>
{
    public static Result<DomainShelf> Map(PersistenceShelf item1, UserVm? item2)
    {
        return DomainShelf.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new ShelfFriendlyId(item1.FriendlyId),
            item2,
            (Name)item1.Name,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            item1.IsLocked);
    }

    public static Result<DomainShelf> Map(PersistenceShelf item1, UserVm? item2, ICollection<DomainBook> item3)
    {
        return DomainShelf.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new ShelfFriendlyId(item1.FriendlyId),
            item2,
            (Name)item1.Name,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            item1.IsLocked,
            item3);
    }

    public static PersistenceShelf Map(DomainShelf item1)
    {
        return new PersistenceShelf()
        {
            Id = item1.Id,
            FriendlyId = item1.FriendlyId.Value,
            CreatorId = item1.CreatorId,
            Name = item1.Name.Value,
            NameNormalized = item1.Name.ValueNormalized,
            Description = item1.Description?.Value,
            IsLocked = item1.IsLocked,
            CreatedOnUtc = item1.CreatedOnUtc,
            ModifiedOnUtc = item1.ModifiedOnUtc
        };
    }

    public static PersistenceShelf Map(DomainShelf item1, GroupId item2)
    {
        var shelf = Map(item1);
        shelf.GroupId = item2;

        return shelf;
    }
}