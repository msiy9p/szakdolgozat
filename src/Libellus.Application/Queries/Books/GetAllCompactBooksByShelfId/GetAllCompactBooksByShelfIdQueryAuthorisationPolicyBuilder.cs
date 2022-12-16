using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Books.GetAllCompactBooksByShelfId;

public sealed class GetAllCompactBooksByShelfIdQueryAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<GetAllCompactBooksByShelfIdQuery>
{
    public override void BuildPolicy(GetAllCompactBooksByShelfIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}