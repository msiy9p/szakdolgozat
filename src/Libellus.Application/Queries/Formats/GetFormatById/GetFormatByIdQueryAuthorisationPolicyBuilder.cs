using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Formats.GetFormatById;

public sealed class
    GetFormatByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetFormatByIdQuery>
{
    public override void BuildPolicy(GetFormatByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}