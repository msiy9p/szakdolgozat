using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Books.GetBookByTitle;

public sealed class
    GetBookByTitleQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetBookByTitleQuery>
{
    public override void BuildPolicy(GetBookByTitleQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}