using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Series.GetSeriesById;

public sealed class
    GetSeriesByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetSeriesByIdQuery>
{
    public override void BuildPolicy(GetSeriesByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}