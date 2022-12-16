using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Labels.GetLabelByName;

public sealed class
    GetLabelByNameQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetLabelByNameQuery>
{
    public override void BuildPolicy(GetLabelByNameQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}