using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Books.GetBookById;

public sealed class
    GetBookByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetBookByIdQuery>
{
    public override void BuildPolicy(GetBookByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}