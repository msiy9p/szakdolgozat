using System.Security.Claims;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace LibellusWeb.Services;

public sealed class ClaimsPrincipalExtractor
{
    private readonly IdentityOptions _options;

    public ClaimsPrincipalExtractor(IOptions<IdentityOptions> optionsAccessor)
    {
        _options = optionsAccessor?.Value ?? new IdentityOptions();
    }

    public Result<UserId> GetUserId(ClaimsPrincipal principal)
    {
        var tempId = principal.FindFirstValue(_options.ClaimsIdentity.UserIdClaimType);
        if (Guid.TryParse(tempId, out var guidId))
        {
            return new UserId(guidId).ToResult();
        }

        return DomainErrors.UserErrors.UserNotFound.ToErrorResult<UserId>();
    }

    public bool IsSignedIn(ClaimsPrincipal principal)
    {
        if (principal is null)
        {
            return false;
        }

        return principal?.Identities != null &&
               principal.Identities.Any(i => i.AuthenticationType == IdentityConstants.ApplicationScheme);
    }
}