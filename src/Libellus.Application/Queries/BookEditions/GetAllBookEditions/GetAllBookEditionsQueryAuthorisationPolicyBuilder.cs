using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.BookEditions.GetAllBookEditions;

public sealed class
    GetAllBookEditionsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetAllBookEditionsQuery>
{
    public override void BuildPolicy(GetAllBookEditionsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}