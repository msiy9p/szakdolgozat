#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;
using NodaTime;
using NpgsqlTypes;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class LiteratureForm : BaseStampedModel<LiteratureFormId, GroupId>, ISearchable
{
    public UserId? CreatorId { get; set; }

    public string Name { get; set; }
    public string NameNormalized { get; set; }
    public decimal ScoreMultiplier { get; set; }

    public List<Book> Books { get; set; } = new List<Book>();

    public NpgsqlTsVector SearchVectorOne { get; set; }
    public NpgsqlTsVector SearchVectorTwo { get; set; }

    public LiteratureForm()
    {
    }

    public LiteratureForm(LiteratureFormId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        UserId? creatorId, string name, string nameNormalized, decimal scoreMultiplier) : base(id, groupId,
        createdOnUtc, modifiedOnUtc)
    {
        CreatorId = creatorId;
        Name = name;
        NameNormalized = nameNormalized;
        ScoreMultiplier = scoreMultiplier;
    }
}