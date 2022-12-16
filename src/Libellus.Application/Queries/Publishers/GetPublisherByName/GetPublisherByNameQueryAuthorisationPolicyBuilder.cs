using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Publishers.GetPublisherByName;

public sealed class
    GetPublisherByNameQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetPublisherByNameQuery>
{
    public override void BuildPolicy(GetPublisherByNameQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}