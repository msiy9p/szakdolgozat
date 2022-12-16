using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Books.GetAllBooksByAuthorIdPaginated;

public sealed class GetAllBooksByAuthorIdPaginatedQueryAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<GetAllBooksByAuthorIdPaginatedQuery>
{
    public override void BuildPolicy(GetAllBooksByAuthorIdPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}