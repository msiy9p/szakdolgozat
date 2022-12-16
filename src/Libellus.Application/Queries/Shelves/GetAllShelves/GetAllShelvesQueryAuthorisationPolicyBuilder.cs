using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Shelves.GetAllShelves;

public sealed class
    GetAllShelvesQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllShelvesQuery>
{
    public override void BuildPolicy(GetAllShelvesQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}