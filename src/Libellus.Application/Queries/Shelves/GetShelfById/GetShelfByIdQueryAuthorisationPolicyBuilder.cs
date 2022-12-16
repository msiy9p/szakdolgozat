using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Shelves.GetShelfById;

public sealed class
    GetShelfByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetShelfByIdQuery>
{
    public override void BuildPolicy(GetShelfByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}