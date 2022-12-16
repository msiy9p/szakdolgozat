using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using DomainLiteratureForm = Libellus.Domain.Entities.LiteratureForm;
using PersistenceLiteratureForm = Libellus.Infrastructure.Persistence.DataModels.LiteratureForm;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct LiteratureFormMapper : IMapFrom<PersistenceLiteratureForm, Result<DomainLiteratureForm>>,
    IMapFrom<DomainLiteratureForm, PersistenceLiteratureForm>,
    IMapFrom<DomainLiteratureForm, GroupId, PersistenceLiteratureForm>
{
    public static Result<DomainLiteratureForm> Map(PersistenceLiteratureForm item1)
    {
        return DomainLiteratureForm.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            item1.CreatorId,
            (ShortName)item1.Name,
            new ScoreMultiplier(item1.ScoreMultiplier));
    }

    public static PersistenceLiteratureForm Map(DomainLiteratureForm item)
    {
        return new PersistenceLiteratureForm()
        {
            Id = item.Id,
            CreatorId = item.CreatorId,
            Name = item.Name.Value,
            NameNormalized = item.Name.ValueNormalized,
            ScoreMultiplier = item.ScoreMultiplier.Value,
            CreatedOnUtc = item.CreatedOnUtc,
            ModifiedOnUtc = item.ModifiedOnUtc
        };
    }

    public static PersistenceLiteratureForm Map(DomainLiteratureForm item1, GroupId item2)
    {
        var literatureForm = Map(item1);
        literatureForm.GroupId = item2;

        return literatureForm;
    }
}