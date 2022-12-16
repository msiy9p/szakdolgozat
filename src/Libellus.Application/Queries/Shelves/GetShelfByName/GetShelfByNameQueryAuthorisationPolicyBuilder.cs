using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Shelves.GetShelfByName;

public sealed class
    GetShelfByNameQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetShelfByNameQuery>
{
    public override void BuildPolicy(GetShelfByNameQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}