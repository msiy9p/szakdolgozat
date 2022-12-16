using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.WarningTags.GetAllWarningTags;

public sealed class
    GetAllWarningTagsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllWarningTagsQuery>
{
    public override void BuildPolicy(GetAllWarningTagsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}