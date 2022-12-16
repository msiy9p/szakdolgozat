using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Publishers.GetPublisherById;

public sealed class
    GetPublisherByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetPublisherByIdQuery>
{
    public override void BuildPolicy(GetPublisherByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}