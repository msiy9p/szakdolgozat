using Ardalis.GuardClauses;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;

namespace Libellus.Domain.ViewModels;

public sealed class UserEmailVm
{
    public string UserEmail { get; init; }
    public string UserName { get; init; }

    public UserEmailVm(string userName, string userEmail)
    {
        UserName = Guard.Against.NullOrWhiteSpace(userName);
        UserEmail = Guard.Against.NullOrWhiteSpace(userEmail);
    }

    public static Result<UserEmailVm> Create(string userName, string userEmail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            return DomainErrors.GeneralErrors.StringNullOrWhiteSpace.ToInvalidResult<UserEmailVm>();
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            return DomainErrors.GeneralErrors.StringNullOrWhiteSpace.ToInvalidResult<UserEmailVm>();
        }

        return new UserEmailVm(userName, userEmail).ToResult();
    }
}