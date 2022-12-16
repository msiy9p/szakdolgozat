using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Authors.SearchAuthors;

public sealed class
    SearchAuthorsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<SearchAuthorsQuery>
{
    public override void BuildPolicy(SearchAuthorsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}