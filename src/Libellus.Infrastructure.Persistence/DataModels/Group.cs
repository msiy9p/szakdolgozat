#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;
using NodaTime;
using NpgsqlTypes;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Group : IId<GroupId>, ISearchable
{
    public GroupId Id { get; set; }
    public string FriendlyId { get; set; }

    public string Name { get; set; }
    public string NameNormalized { get; set; }
    public string? Description { get; set; }
    public bool IsPrivate { get; set; }
    public ZonedDateTime CreatedOnUtc { get; set; }
    public ZonedDateTime ModifiedOnUtc { get; set; }

    public List<Label> Labels { get; set; } = new List<Label>();
    public List<Post> Posts { get; set; } = new List<Post>();
    public List<GroupUserMembership> Members { get; set; } = new List<GroupUserMembership>();

    public NpgsqlTsVector SearchVectorOne { get; set; }
    public NpgsqlTsVector SearchVectorTwo { get; set; }

    public Group()
    {
    }

    public Group(GroupId id, string friendlyId, string name, string nameNormalized,
        string description, bool isPrivate, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc)
    {
        Id = id;
        FriendlyId = friendlyId;
        Name = name;
        NameNormalized = nameNormalized;
        Description = description;
        IsPrivate = isPrivate;
        CreatedOnUtc = createdOnUtc;
        ModifiedOnUtc = modifiedOnUtc;
    }
}