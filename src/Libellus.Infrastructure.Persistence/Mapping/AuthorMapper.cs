using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using DomainAuthor = Libellus.Domain.Entities.Author;
using DomainCoverImageMetaData = Libellus.Domain.Entities.CoverImageMetaData;
using PersistenceAuthor = Libellus.Infrastructure.Persistence.DataModels.Author;
using PersistenceCoverImage = Libellus.Infrastructure.Persistence.DataModels.CoverImageMetaData;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct AuthorMapper :
    IMapFrom<PersistenceAuthor, UserVm?, ICollection<PersistenceCoverImage>, Result<DomainAuthor>>,
    IMapFrom<PersistenceAuthor, UserVm?, ICollection<DomainCoverImageMetaData>, Result<DomainAuthor>>,
    IMapFrom<DomainAuthor, PersistenceAuthor>, IMapFrom<DomainAuthor, GroupId, PersistenceAuthor>
{
    public static Result<DomainAuthor> Map(PersistenceAuthor item1, UserVm? item2,
        ICollection<DomainCoverImageMetaData> item3)
    {
        if (item1.CoverImageId.HasValue)
        {
            return DomainAuthor.Create(
                item1.Id,
                item1.CreatedOnUtc,
                item1.ModifiedOnUtc,
                new AuthorFriendlyId(item1.FriendlyId),
                item2,
                (Name)item1.Name,
                new CoverImageMetaDataContainer(item1.CoverImageId!.Value, item3));
        }

        return DomainAuthor.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new AuthorFriendlyId(item1.FriendlyId),
            item2,
            (Name)item1.Name,
            null);
    }

    public static Result<DomainAuthor> Map(PersistenceAuthor item1, UserVm? item2,
        ICollection<PersistenceCoverImage> item3)
    {
        if (item1.CoverImageId.HasValue)
        {
            var imgMeta = new List<DomainCoverImageMetaData>(item3.Count);
            foreach (var item in item3)
            {
                var temp = CoverImageMetaDataMapper.Map(item);

                if (temp.IsSuccess)
                {
                    imgMeta.Add(temp.Value!);
                }
            }

            return DomainAuthor.Create(
                item1.Id,
                item1.CreatedOnUtc,
                item1.ModifiedOnUtc,
                new AuthorFriendlyId(item1.FriendlyId),
                item2,
                (Name)item1.Name,
                new CoverImageMetaDataContainer(item1.CoverImageId!.Value, imgMeta));
        }

        return DomainAuthor.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new AuthorFriendlyId(item1.FriendlyId),
            item2,
            (Name)item1.Name,
            null);
    }

    public static PersistenceAuthor Map(DomainAuthor item1)
    {
        return new PersistenceAuthor()
        {
            Id = item1.Id,
            FriendlyId = item1.FriendlyId.Value,
            CreatorId = item1.CreatorId,
            CoverImageId = item1.AvailableCovers?.Id,
            Name = item1.Name.Value,
            NameNormalized = item1.Name.ValueNormalized,
            CreatedOnUtc = item1.CreatedOnUtc,
            ModifiedOnUtc = item1.ModifiedOnUtc,
        };
    }

    public static PersistenceAuthor Map(DomainAuthor item1, GroupId item2)
    {
        var author = Map(item1);
        author.GroupId = item2;

        return author;
    }
}