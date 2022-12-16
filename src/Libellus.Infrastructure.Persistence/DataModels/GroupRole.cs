#pragma warning disable CS8618
using Libellus.Domain.Utilities;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class GroupRole
{
    public Guid GroupRoleId { get; set; }
    public string Name { get; set; }
    public string NameNormalized { get; set; }

    public List<GroupUserMembership> GroupUserMemberships { get; set; } = new List<GroupUserMembership>();

    public GroupRole()
    {
    }

    public GroupRole(string name) : this(Guid.NewGuid(), name)
    {
    }

    public GroupRole(Guid groupRoleId, string name)
    {
        GroupRoleId = groupRoleId;
        Name = name;
        NameNormalized = Name.ToNormalizedUpperInvariant();
    }

    public GroupRole(Guid groupRoleId, string name, string nameNormalized)
    {
        GroupRoleId = groupRoleId;
        Name = name;
        NameNormalized = nameNormalized;
    }

    public override string ToString() => Name;
}