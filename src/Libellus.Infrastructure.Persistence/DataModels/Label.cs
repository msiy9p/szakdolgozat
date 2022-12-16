#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;
using NodaTime;
using NpgsqlTypes;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Label : BaseStampedModel<LabelId, GroupId>, ISearchable
{
    public string Name { get; set; }
    public string NameNormalized { get; set; }

    public List<Post> Posts { get; set; } = new List<Post>();

    public NpgsqlTsVector SearchVectorOne { get; set; }
    public NpgsqlTsVector SearchVectorTwo { get; set; }

    public Label()
    {
    }

    public Label(LabelId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, string name,
        string nameNormalized) : base(id, groupId, createdOnUtc, modifiedOnUtc)
    {
        Name = name;
        NameNormalized = nameNormalized;
    }
}