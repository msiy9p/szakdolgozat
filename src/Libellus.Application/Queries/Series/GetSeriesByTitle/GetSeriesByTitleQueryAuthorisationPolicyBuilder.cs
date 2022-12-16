using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Series.GetSeriesByTitle;

public sealed class
    GetSeriesByTitleQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetSeriesByTitleQuery>
{
    public override void BuildPolicy(GetSeriesByTitleQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}