using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Shelves.GetAllShelvesPaginated;

public sealed class
    GetAllShelvesPaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetAllShelvesPaginatedQuery>
{
    public override void BuildPolicy(GetAllShelvesPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}