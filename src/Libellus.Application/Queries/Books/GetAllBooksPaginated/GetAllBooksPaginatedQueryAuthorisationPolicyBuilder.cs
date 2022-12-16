using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Books.GetAllBooksPaginated;

public sealed class
    GetAllBooksPaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetAllBooksPaginatedQuery>
{
    public override void BuildPolicy(GetAllBooksPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}