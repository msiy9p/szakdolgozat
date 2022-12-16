using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Series.GetSeriesByTitlePaginated;

public sealed class
    GetSeriesByTitlePaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetSeriesByTitlePaginatedQuery>
{
    public override void BuildPolicy(GetSeriesByTitlePaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}