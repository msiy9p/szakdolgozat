using Ardalis.GuardClauses;
using Libellus.Domain.Common.Errors;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Entities.Identity;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;

namespace Libellus.Domain.ViewModels;

public sealed class UserPicturedVm : IEquatable<UserPicturedVm>
{
    public UserId UserId { get; init; }
    public string UserName { get; init; }
    public ProfilePictureMetaDataContainer? AvailableProfilePictures { get; private set; }

    internal UserPicturedVm(UserId userId, string userName, ProfilePictureMetaDataContainer? availableProfilePictures)
    {
        UserId = userId;
        UserName = Guard.Against.NullOrWhiteSpace(userName);
        if (availableProfilePictures is not null)
        {
            Guard.Against.NegativeOrZero(availableProfilePictures.Count);
        }

        AvailableProfilePictures = availableProfilePictures;
    }

    internal UserPicturedVm(User user)
    {
        Guard.Against.Null(user);
        Guard.Against.NullOrWhiteSpace(user.UserName);
        if (user.AvailableProfilePictures is not null)
        {
            Guard.Against.NegativeOrZero(user.AvailableProfilePictures.Count);
        }

        UserId = user.Id;
        UserName = user.UserName;
        AvailableProfilePictures = user.AvailableProfilePictures;
    }

    public static Result<UserPicturedVm> Create(UserId userId, string userName,
        ProfilePictureMetaDataContainer? availableProfilePictures)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return DomainErrors.GeneralErrors.StringNullOrWhiteSpace.ToInvalidResult<UserPicturedVm>();
        }

        if (availableProfilePictures is not null && availableProfilePictures.Count < 1)
        {
            return new Error("ProfilePictureNotValid", "AvailableProfilePictures not valid.")
                .ToInvalidResult<UserPicturedVm>();
        }

        return new UserPicturedVm(userId, userName, availableProfilePictures).ToResult();
    }

    public static Result<UserPicturedVm> Create(User user)
    {
        if (user is null)
        {
            return DomainErrors.GeneralErrors.InputIsNull.ToInvalidResult<UserPicturedVm>();
        }

        if (!string.IsNullOrWhiteSpace(user!.UserName))
        {
            return DomainErrors.GeneralErrors.StringNullOrWhiteSpace.ToInvalidResult<UserPicturedVm>();
        }

        if (user.AvailableProfilePictures is not null && user.AvailableProfilePictures.Count < 1)
        {
            return new Error("ProfilePictureNotValid", "AvailableProfilePictures not valid.")
                .ToInvalidResult<UserPicturedVm>();
        }

        return new UserPicturedVm(user).ToResult();
    }

    public static UserPicturedVm? Convert(UserId? userId, string? userName,
        ProfilePictureMetaDataContainer? availableProfilePictures)
    {
        if (!userId.HasValue)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            return null;
        }

        if (availableProfilePictures is not null && availableProfilePictures.Count < 1)
        {
            return null;
        }

        return new UserPicturedVm(userId.Value, userName, availableProfilePictures);
    }

    public static explicit operator UserVm(UserPicturedVm item) => new(item.UserId, item.UserName);

    public static explicit operator UserPicturedVm?(UserId? userId) =>
        userId.HasValue ? new UserPicturedVm(userId.Value, userId.Value.ToString(), null) : null;

    public bool Equals(UserPicturedVm? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return UserId.Equals(other.UserId);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is UserPicturedVm other && Equals(other);
    }

    public override int GetHashCode()
    {
        return UserId.GetHashCode();
    }

    public override string ToString()
    {
        return UserName;
    }
}