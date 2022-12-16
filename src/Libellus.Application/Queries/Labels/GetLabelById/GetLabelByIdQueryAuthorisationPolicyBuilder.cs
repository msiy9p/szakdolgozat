using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Labels.GetLabelById;

public sealed class
    GetLabelByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetLabelByIdQuery>
{
    public override void BuildPolicy(GetLabelByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}