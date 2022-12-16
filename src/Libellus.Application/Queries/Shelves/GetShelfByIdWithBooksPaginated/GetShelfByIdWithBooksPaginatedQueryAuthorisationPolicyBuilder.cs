using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Shelves.GetShelfByIdWithBooksPaginated;

public sealed class GetShelfByIdWithBooksPaginatedQueryAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<GetShelfByIdWithBooksPaginatedQuery>
{
    public override void BuildPolicy(GetShelfByIdWithBooksPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}