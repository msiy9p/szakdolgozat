using Libellus.Domain.Common.Errors;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Security;

public static class SecurityConstants
{
    public static class IdentityRoles
    {
        public const string Administrator = "Administrator";
        public const string User = "User";
    }

    public static class GroupRoles
    {
        public const string Owner = "Owner";
        public const string Moderator = "Moderator";
        public const string Member = "Member";
    }

    public static class AuthorisationResults
    {
        public static readonly Result CurrentUserNotFound =
            Result.Error(new Error(nameof(CurrentUserNotFound), "No current user found."));

        public static readonly Result CurrentGroupNotFound =
            Result.Error(new Error(nameof(CurrentGroupNotFound), "No current group found."));

        public static readonly Result NotTheSameUser =
            Result.Error(new Error(nameof(NotTheSameUser), "Not the same user."));

        public static readonly Result NotInCurrentGroup =
            Result.Error(new Error(nameof(NotInCurrentGroup), "Not in the current group."));

        public static readonly Result NotAnOwner =
            Result.Error(new Error(nameof(NotAnOwner), "Not an owner in current group."));

        public static readonly Result NotAModerator =
            Result.Error(new Error(nameof(NotAModerator), "Not a moderator in current group."));

        public static readonly Result NotAMember =
            Result.Error(new Error(nameof(NotAMember), "Not a member in current group."));

        public static readonly Result NoUserFound =
            Result.Error(new Error(nameof(NoUserFound), "No user found."));

        public static readonly Result NotOwnerOrAbove =
            Result.Error(new Error(nameof(NotOwnerOrAbove), "Not owner or above in current group."));

        public static readonly Result NotModeratorOrAbove =
            Result.Error(new Error(nameof(NotModeratorOrAbove), "Not moderator or above in current group."));

        public static readonly Result NotCreatorOfResource =
            Result.Error(new Error(nameof(NotCreatorOfResource), "Not the creator of this resource."));
    }
}