using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Series.GetAllSeriesPaginated;

public sealed class GetAllSeriesPaginatedQueryAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<GetAllSeriesPaginatedQuery>
{
    public override void BuildPolicy(GetAllSeriesPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}