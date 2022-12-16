using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Books.GetAllBooksByAuthorId;

public sealed class
    GetAllBooksByAuthorIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetAllBooksByAuthorIdQuery>
{
    public override void BuildPolicy(GetAllBooksByAuthorIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}