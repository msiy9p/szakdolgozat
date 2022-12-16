using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Domain.Entities.Identity;

public sealed class Role
{
    public UserId Id { get; init; }

    public UserName Name { get; private set; }

    public string? SecurityStamp { get; private set; }

    public Role(UserId id, UserName name, string? securityStamp)
    {
        Id = id;
        Name = name;
        SecurityStamp = securityStamp;
    }
}