using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Authors.GetAuthorById;

public sealed class
    GetAuthorByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAuthorByIdQuery>
{
    public override void BuildPolicy(GetAuthorByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}