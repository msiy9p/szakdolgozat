using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionByIdWithReadings;

public sealed class
    GetBookEditionByIdWithReadingsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetBookEditionByIdWithReadingsQuery>
{
    public override void BuildPolicy(GetBookEditionByIdWithReadingsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}