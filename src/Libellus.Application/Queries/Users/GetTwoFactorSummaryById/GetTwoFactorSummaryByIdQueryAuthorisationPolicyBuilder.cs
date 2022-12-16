using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Users.GetTwoFactorSummaryById;

public sealed class
    GetTwoFactorSummaryByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetTwoFactorSummaryByIdQuery>
{
    public override void BuildPolicy(GetTwoFactorSummaryByIdQuery instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}