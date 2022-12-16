using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Shelves.GetAllShelvesByBookId;

public sealed class GetAllShelvesByBookIdQueryAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<GetAllShelvesByBookIdQuery>
{
    public override void BuildPolicy(GetAllShelvesByBookIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}