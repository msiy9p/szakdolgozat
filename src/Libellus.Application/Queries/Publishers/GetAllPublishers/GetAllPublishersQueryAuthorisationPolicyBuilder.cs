using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Publishers.GetAllPublishers;

public sealed class
    GetAllPublishersQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllPublishersQuery>
{
    public override void BuildPolicy(GetAllPublishersQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}