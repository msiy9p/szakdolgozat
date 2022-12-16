using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Authors.GetAllAuthors;

public sealed class
    GetAllAuthorsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllAuthorsQuery>
{
    public override void BuildPolicy(GetAllAuthorsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}