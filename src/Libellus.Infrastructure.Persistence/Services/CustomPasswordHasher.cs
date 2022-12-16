using Libellus.Application.Common.Interfaces.Services;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Libellus.Infrastructure.Persistence.Services;

internal sealed class CustomPasswordHasher : PasswordHasher<ApplicationUser>, IPasswordHasher
{
    public CustomPasswordHasher(IOptions<PasswordHasherOptions>? optionsAccessor = null) : base(optionsAccessor)
    {
    }

    public string HashPassword(string password)
    {
        return base.HashPassword(null, password);
    }

    public bool VerifyPassword(string hashedPassword, string plainPassword)
    {
        var result = base.VerifyHashedPassword(null, hashedPassword, plainPassword);

        return result switch
        {
            PasswordVerificationResult.Failed => false,
            PasswordVerificationResult.Success => true,
            PasswordVerificationResult.SuccessRehashNeeded => true,
            _ => false
        };
    }
}