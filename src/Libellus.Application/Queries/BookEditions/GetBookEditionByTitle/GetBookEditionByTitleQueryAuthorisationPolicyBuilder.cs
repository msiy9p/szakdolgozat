using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionByTitle;

public sealed class
    GetBookEditionByTitleQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetBookEditionByTitleQuery>
{
    public override void BuildPolicy(GetBookEditionByTitleQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}