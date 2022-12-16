using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.AspNetCore.Identity;

namespace Libellus.Infrastructure.Persistence.Services;

internal sealed class CustomPasswordValidator : PasswordValidator<ApplicationUser>
{
    public override async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user,
        string? password)
    {
        var result = await base.ValidateAsync(manager, user, password);

        if (user is null || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(password))
        {
            return result;
        }

        var errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

        if (password.ToUpperInvariant().Contains(user.UserName.ToUpperInvariant()))
        {
            errors.Add(new IdentityError() { Code = string.Empty, Description = "Password cannot contain username." });
        }

        return errors.Any() ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }
}