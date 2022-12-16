using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Authors.GetAllAuthorsPaginated;

public sealed class
    GetAllAuthorsPaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetAllAuthorsPaginatedQuery>
{
    public override void BuildPolicy(GetAllAuthorsPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}