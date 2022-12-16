using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Series.SearchSeriesPaginated;

public sealed class
    SearchSeriesPaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        SearchSeriesPaginatedQuery>
{
    public override void BuildPolicy(SearchSeriesPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}