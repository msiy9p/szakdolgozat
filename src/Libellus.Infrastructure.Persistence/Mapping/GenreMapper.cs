using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using DomainGenre = Libellus.Domain.Entities.Genre;
using PersistenceGenre = Libellus.Infrastructure.Persistence.DataModels.Genre;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct GenreMapper : IMapFrom<PersistenceGenre, Result<DomainGenre>>,
    IMapFrom<DomainGenre, PersistenceGenre>, IMapFrom<DomainGenre, GroupId, PersistenceGenre>
{
    public static Result<DomainGenre> Map(PersistenceGenre item1)
    {
        return DomainGenre.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            item1.CreatorId,
            (ShortName)item1.Name,
            item1.IsFiction);
    }

    public static PersistenceGenre Map(DomainGenre item)
    {
        return new PersistenceGenre()
        {
            Id = item.Id,
            CreatorId = item.CreatorId,
            Name = item.Name.Value,
            NameNormalized = item.Name.ValueNormalized,
            IsFiction = item.IsFiction,
            CreatedOnUtc = item.CreatedOnUtc,
            ModifiedOnUtc = item.ModifiedOnUtc
        };
    }

    public static PersistenceGenre Map(DomainGenre item1, GroupId item2)
    {
        var genre = Map(item1);
        genre.GroupId = item2;

        return genre;
    }
}