using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Tags.GetTagById;

public sealed class
    GetTagByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetTagByIdQuery>
{
    public override void BuildPolicy(GetTagByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}