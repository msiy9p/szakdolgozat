using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Readings.GetAllReadingsPaginated;

public sealed class
    GetAllReadingsPaginatedQueryAuthorisationPolicyBuilder :
        BaseAuthorisationPolicyBuilder<GetAllReadingsPaginatedQuery>
{
    public override void BuildPolicy(GetAllReadingsPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}