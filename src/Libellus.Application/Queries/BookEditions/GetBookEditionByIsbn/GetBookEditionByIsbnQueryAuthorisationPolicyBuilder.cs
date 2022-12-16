using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionByIsbn;

public sealed class
    GetBookEditionByIsbnQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetBookEditionByIsbnQuery>
{
    public override void BuildPolicy(GetBookEditionByIsbnQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}