using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionById;

public sealed class
    GetBookEditionByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetBookEditionByIdQuery>
{
    public override void BuildPolicy(GetBookEditionByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}