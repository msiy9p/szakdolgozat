using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.WarningTags.GetWarningTagByName;

public sealed class
    GetWarningTagByNameQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetWarningTagByNameQuery>
{
    public override void BuildPolicy(GetWarningTagByNameQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}