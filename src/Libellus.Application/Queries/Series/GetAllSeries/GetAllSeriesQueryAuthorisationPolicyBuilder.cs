using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Series.GetAllSeries;

public sealed class
    GetAllSeriesQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllSeriesQuery>
{
    public override void BuildPolicy(GetAllSeriesQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}