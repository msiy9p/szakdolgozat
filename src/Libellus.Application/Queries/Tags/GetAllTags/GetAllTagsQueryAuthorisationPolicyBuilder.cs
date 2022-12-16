using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Tags.GetAllTags;

public sealed class
    GetAllTagsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllTagsQuery>
{
    public override void BuildPolicy(GetAllTagsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}