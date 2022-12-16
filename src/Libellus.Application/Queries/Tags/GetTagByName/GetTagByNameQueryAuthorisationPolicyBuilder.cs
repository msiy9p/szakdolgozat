using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Tags.GetTagByName;

public sealed class
    GetTagByNameQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetTagByNameQuery>
{
    public override void BuildPolicy(GetTagByNameQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}