using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Authors.GetAuthorByName;

public sealed class
    GetAuthorByNameQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAuthorByNameQuery>
{
    public override void BuildPolicy(GetAuthorByNameQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}