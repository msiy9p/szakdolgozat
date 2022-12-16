using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Users.IsTwoFactorEnabledById;

public sealed class
    IsTwoFactorEnabledByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        IsTwoFactorEnabledByIdQuery>
{
    public override void BuildPolicy(IsTwoFactorEnabledByIdQuery instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}