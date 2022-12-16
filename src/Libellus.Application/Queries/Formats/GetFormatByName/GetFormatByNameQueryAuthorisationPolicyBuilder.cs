using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Formats.GetFormatByName;

public sealed class
    GetFormatByNameQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetFormatByNameQuery>
{
    public override void BuildPolicy(GetFormatByNameQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}