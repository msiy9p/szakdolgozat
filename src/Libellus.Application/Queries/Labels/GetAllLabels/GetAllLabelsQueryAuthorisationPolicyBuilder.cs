using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Labels.GetAllLabels;

public sealed class
    GetAllLabelsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllLabelsQuery>
{
    public override void BuildPolicy(GetAllLabelsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}