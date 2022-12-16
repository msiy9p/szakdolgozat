using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Authors.GetAuthorByNamePaginated;

public sealed class
    GetAuthorByNamePaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetAuthorByNamePaginatedQuery>
{
    public override void BuildPolicy(GetAuthorByNamePaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}