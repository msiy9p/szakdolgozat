using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Formats.GetAllFormats;

public sealed class
    GetAllFormatsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllFormatsQuery>
{
    public override void BuildPolicy(GetAllFormatsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}