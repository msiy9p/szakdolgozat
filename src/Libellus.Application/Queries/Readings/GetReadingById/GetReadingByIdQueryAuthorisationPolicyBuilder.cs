using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Readings.GetReadingById;

public sealed class
    GetReadingByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetReadingByIdQuery>
{
    public override void BuildPolicy(GetReadingByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}