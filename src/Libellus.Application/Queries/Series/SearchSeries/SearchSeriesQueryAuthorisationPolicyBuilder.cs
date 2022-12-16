using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Series.SearchSeries;

public sealed class
    SearchSeriesQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<SearchSeriesQuery>
{
    public override void BuildPolicy(SearchSeriesQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}