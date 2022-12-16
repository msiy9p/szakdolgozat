using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Books.GetBookByIdWithBookEdition;

public sealed class GetBookByIdWithBookEditionQueryAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<GetBookByIdWithBookEditionQuery>
{
    public override void BuildPolicy(GetBookByIdWithBookEditionQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}