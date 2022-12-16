using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using DomainLanguage = Libellus.Domain.Entities.Language;
using PersistenceLanguage = Libellus.Infrastructure.Persistence.DataModels.Language;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct LanguageMapper : IMapFrom<PersistenceLanguage, Result<DomainLanguage>>,
    IMapFrom<DomainLanguage, PersistenceLanguage>, IMapFrom<DomainLanguage, GroupId, PersistenceLanguage>
{
    public static Result<DomainLanguage> Map(PersistenceLanguage item1)
    {
        return DomainLanguage.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            item1.CreatorId,
            (ShortName)item1.Name);
    }

    public static PersistenceLanguage Map(DomainLanguage item)
    {
        return new PersistenceLanguage()
        {
            Id = item.Id,
            CreatorId = item.CreatorId,
            Name = item.Name.Value,
            NameNormalized = item.Name.ValueNormalized,
            CreatedOnUtc = item.CreatedOnUtc,
            ModifiedOnUtc = item.ModifiedOnUtc
        };
    }

    public static PersistenceLanguage Map(DomainLanguage item1, GroupId item2)
    {
        var language = Map(item1);
        language.GroupId = item2;

        return language;
    }
}