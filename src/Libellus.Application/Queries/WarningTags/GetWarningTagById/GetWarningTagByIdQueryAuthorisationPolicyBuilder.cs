using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.WarningTags.GetWarningTagById;

public sealed class
    GetWarningTagByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetWarningTagByIdQuery>
{
    public override void BuildPolicy(GetWarningTagByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}