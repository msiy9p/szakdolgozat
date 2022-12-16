using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Series.GetSeriesByIdWithBooksPaginated;

public sealed class GetSeriesByIdWithBooksPaginatedQueryAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<GetSeriesByIdWithBooksPaginatedQuery>
{
    public override void BuildPolicy(GetSeriesByIdWithBooksPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}