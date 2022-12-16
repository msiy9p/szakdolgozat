using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;

namespace Libellus.Domain.Enums;

public static class GroupRoleExtensions
{
    public static bool IsDefined(GroupRole groupRole) =>
        groupRole switch
        {
            GroupRole.Member => true,
            GroupRole.Moderator => true,
            GroupRole.Owner => true,
            _ => false
        };

    public static string ToString(GroupRole groupRole) =>
        groupRole switch
        {
            GroupRole.Member => nameof(GroupRole.Member),
            GroupRole.Moderator => nameof(GroupRole.Moderator),
            GroupRole.Owner => nameof(GroupRole.Owner),
            _ => string.Empty
        };

    public static Result<GroupRole> FromString(string groupRole) =>
        groupRole.ToLowerInvariant() switch
        {
            "member" => GroupRole.Member.ToResult(),
            "moderator" => GroupRole.Moderator.ToResult(),
            "owner" => GroupRole.Owner.ToResult(),
            _ => DomainErrors.GroupRoleErrors.InvalidGroupRole.ToErrorResult<GroupRole>()
        };
}