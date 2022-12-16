using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Domain.Entities.Identity;

public sealed class User : BaseEntity<UserId>
{
    private ProfilePictureId? _temProfilePictureId;

    public ProfilePictureId? ProfilePictureId
    {
        get => _temProfilePictureId;
        set => _temProfilePictureId = value;
    }

    public ProfilePictureMetaDataContainer? AvailableProfilePictures { get; private set; }

    public UserName UserName { get; init; }

    public Email Email { get; private set; }

    public bool EmailConfirmed { get; private set; } = false;

    public string? PasswordHash { get; private set; }

    public string? SecurityStamp { get; private set; }

    public bool TwoFactorEnabled { get; private set; } = false;

    public DateTimeOffset? LockoutEnd { get; private set; } = null;

    public bool LockoutEnabled { get; private set; } = true;

    public int AccessFailedCount { get; private set; } = 0;

    public User(UserId id, UserName userName, Email email, bool emailConfirmed, string? passwordHash,
        string? securityStamp, bool twoFactorEnabled, DateTimeOffset? lockoutEnd, bool lockoutEnabled,
        int accessFailedCount) : base(id)
    {
        UserName = userName;
        Email = email;
        EmailConfirmed = emailConfirmed;
        PasswordHash = passwordHash;
        SecurityStamp = securityStamp;
        TwoFactorEnabled = twoFactorEnabled;
        LockoutEnd = lockoutEnd;
        LockoutEnabled = lockoutEnabled;
        AccessFailedCount = accessFailedCount;
    }

    public bool RemoveProfilePictures()
    {
        AvailableProfilePictures = null;
        _temProfilePictureId = null;

        return true;
    }

    public bool ChangeProfilePictures(ProfilePictureMetaDataContainer availablePictures)
    {
        if (availablePictures is null || availablePictures.Count < 1)
        {
            return false;
        }

        AvailableProfilePictures = availablePictures;
        _temProfilePictureId = AvailableProfilePictures.Id;

        return true;
    }

    public override string ToString() => UserName.ToString();
}