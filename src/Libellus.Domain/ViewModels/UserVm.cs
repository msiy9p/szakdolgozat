using Ardalis.GuardClauses;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities.Identity;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;

namespace Libellus.Domain.ViewModels;

public sealed class UserVm : IEquatable<UserVm>
{
    public UserId UserId { get; init; }
    public string UserName { get; init; }

    internal UserVm(UserId userId, string userName)
    {
        UserId = userId;
        UserName = Guard.Against.NullOrWhiteSpace(userName);
    }

    internal UserVm(User user)
    {
        Guard.Against.Null(user);
        Guard.Against.NullOrWhiteSpace(user.UserName);

        UserId = user.Id;
        UserName = user.UserName;
    }

    public static Result<UserVm> Create(UserId userId, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return DomainErrors.GeneralErrors.StringNullOrWhiteSpace.ToInvalidResult<UserVm>();
        }

        return new UserVm(userId, userName).ToResult();
    }

    public static Result<UserVm> Create(User user)
    {
        if (user is null)
        {
            return DomainErrors.GeneralErrors.InputIsNull.ToInvalidResult<UserVm>();
        }

        if (!string.IsNullOrWhiteSpace(user!.UserName))
        {
            return DomainErrors.GeneralErrors.StringNullOrWhiteSpace.ToInvalidResult<UserVm>();
        }

        return new UserVm(user).ToResult();
    }

    public static UserVm? Convert(UserId? userId, string? userName)
    {
        if (!userId.HasValue)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            return null;
        }

        return new UserVm(userId.Value, userName);
    }

    public static explicit operator UserVm?(UserId? userId) =>
        userId.HasValue ? new UserVm(userId.Value, userId.Value.ToString()) : null;

    public bool Equals(UserVm? other)
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
        return ReferenceEquals(this, obj) || obj is UserVm other && Equals(other);
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